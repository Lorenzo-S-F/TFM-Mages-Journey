using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private List<BoardElement> m_BoardElements;

    private bool [,] m_OccupationData;

    [SerializeField]
    private PlayerController m_PlayerController;

    public Transform m_ProjectilesTransform;
    public Transform m_BoardTransform;

    [SerializeField]
    private int m_BoardSizeX;
    [SerializeField]
    private int m_BoardSizeY;
    [SerializeField]
    private bool m_AutoAimMode;

    [SerializeField]
    private bool m_AutoAimPanel;
    [SerializeField]
    private bool m_ManualAimPanel;

    public Player m_PlayerBase;
    private RoomManager m_CurrentRoom;
    private bool m_RoomStarted = false;

    private void Update()
    {
        if (m_RoomStarted && (m_CurrentRoom.GetRoomType() == LevelManager.MapNode.ROOM_TYPE.ENEMY || m_CurrentRoom.GetRoomType() == LevelManager.MapNode.ROOM_TYPE.BOSS))
        {
            int entity = m_BoardElements.FindIndex(x => x != null && x.GetElementType() == BoardElement.ELEMENT_TYPE.ENEMY);
            if (entity == -1)
            {
                m_RoomStarted = false;
                StartCoroutine(GameplayManagers.Instance.ShowNextRoomOptions());

                foreach (Transform projectile in m_ProjectilesTransform)
                {
                    Destroy(projectile.gameObject);
                }
            }

            if (m_PlayerBase == null)
                MainManagers.Instance.m_LoadingHandler.LoadScene(LoadingHandler.SCENE.MENUS);
        }
    }

    public void ApplyItemToPlayer(ItemSlotHandler.Item item)
    {
        m_PlayerBase.ApplyItem(item);
    }
    public bool IsAnyObstacle(int iterations, Vector2Int direction, Vector2Int startPos)
    {
        for (int i = 0; i < iterations; ++i)
        {
            startPos += direction;
            if (m_OccupationData[startPos.x, startPos.y])
                return true;
        }
        return false;
    }

    internal RoomEntity GetPlayerEntity()
    {
        return m_PlayerBase.GetRoomEntity();
    }

    internal void InitializeGame(RoomManager room, LevelManager.MapNode node, RoomEntity player)
    {
        foreach(Transform projectile in m_ProjectilesTransform)
        {
            Destroy(projectile.gameObject);
        }

        m_BoardElements.Clear();

        m_BoardSizeX = room.GetRoomSize().x;
        m_BoardSizeY = room.GetRoomSize().y;

        m_OccupationData = new bool[m_BoardSizeX, m_BoardSizeY];

        RoomManager.ValidEncounter encounter = room.GetEncounter(node.m_Rarity);
        SetupRoom(room, player, encounter);
    }

    public bool IsValidPosition(int x, int y)
    {
        if (x < 0 || x >= m_BoardSizeX || y < 0 || y >= m_BoardSizeY || m_OccupationData[x, y] || m_BoardElements.FindIndex(element => element != null && element.GetPosition().x == x && element.GetPosition().y == y) != -1)
            return false;

        return true;
    }

    internal Vector2Int GetMostAlignedDirection(BoardElement element)
    {
        float minDist = float.MaxValue;
        BoardElement closestElement = null;
        bool selectedIsAligned = false;

        foreach(var entity in m_BoardElements)
        {
            if (entity == null)
                continue;

            if (selectedIsAligned && entity.GetPosition().x != element.GetPosition().x && entity.GetPosition().y != element.GetPosition().y)
                continue;

            bool entityAligned = entity.GetPosition().x != element.GetPosition().x || entity.GetPosition().y != element.GetPosition().y;

            float dist = Vector2.Distance(entity.GetPosition(), element.GetPosition());

            if (dist < minDist || (!selectedIsAligned && entityAligned))
            {
                selectedIsAligned = entityAligned;
                closestElement = entity;
                minDist = dist;
            }
        }

        Vector2 dir = new Vector2Int(closestElement.GetPosition().x - element.GetPosition().x, closestElement.GetPosition().y - element.GetPosition().y);
        dir = dir.normalized;

        if (Math.Abs(dir.x) > Math.Abs(dir.y))
        {
            if (dir.x > 0)
                return new Vector2Int(1, 0);
            else
                return new Vector2Int(-1, 0);
        }
        else
        {
            if (dir.y > 0)
                return new Vector2Int(0, 1);
            else
                return new Vector2Int(0, -1);
        }
    }

    internal Vector2Int GetPlayerDirection(BoardElement element)
    {
        BoardElement player = m_PlayerBase;

        Vector2 dir = new Vector2Int(player.GetPosition().x - element.GetPosition().x, player.GetPosition().y - element.GetPosition().y);
        dir = dir.normalized;

        if (Math.Abs(dir.x) > Math.Abs(dir.y))
        {
            if (dir.x > 0)
                return new Vector2Int(1, 0);
            else
                return new Vector2Int(-1, 0);
        }
        else
        {
            if (dir.y > 0)
                return new Vector2Int(0, 1);
            else
                return new Vector2Int(0, -1);
        }
    }

    public void SetupRoom(RoomManager roomManager, RoomEntity player, RoomManager.ValidEncounter encounter)
    {
        if (m_CurrentRoom != null)
        {
            Destroy(m_CurrentRoom.gameObject);
            m_CurrentRoom = null;
        }

        m_CurrentRoom = Instantiate(roomManager, m_BoardTransform);
        m_CurrentRoom.Initialize();

        GameObject playerObject = Instantiate(player.m_EntityGameObject, m_CurrentRoom.GetPlayerTransform());
        playerObject.transform.SetParent(m_CurrentRoom.GetPlayerTransform());
        m_PlayerBase.InitializeTransform(playerObject.transform);
        m_PlayerController.SetPlayer(m_PlayerBase);
        InitializeBoardElement(m_PlayerBase, encounter.m_PlayerInitialState.x, encounter.m_PlayerInitialState.y, player);

        foreach (var entity in encounter.m_RoomEnemies)
        {
            switch (entity.m_EnemyData.m_Type)
            {
                case RoomEntity.ENTITY_TYPE.OBSTACLE:
                    m_OccupationData[entity.m_StartPosition.x, entity.m_StartPosition.y] = true;
                    break;
                case RoomEntity.ENTITY_TYPE.ENEMY:
                case RoomEntity.ENTITY_TYPE.NPC:
                    Enemy enemy = Instantiate(entity.m_EnemyData.m_EntityGameObject, m_CurrentRoom.GetGridTransform()).GetComponent<Enemy>();
                    InitializeBoardElement(enemy, entity.m_StartPosition.x, entity.m_StartPosition.y, entity.m_EnemyData);
                    break;
            }
        }

        m_RoomStarted = true;
    }

    public void InitializeBoardElement(BoardElement element, int x, int y, RoomEntity entity)
    {
        m_BoardElements.Add(element);
        entity.m_Position = new Vector2Int(x, y);
        element.Initialize(entity);
        SetBoardElementPosition(element, x, y);
    }

    private void SetBoardElementPosition(BoardElement element, int x, int y)
    {
        element.SetPosition(x - (m_BoardSizeX - 1) / 2, y - (m_BoardSizeY + 2) / 2);
    }

    public bool IsPositionValid(int x, int y, bool isPlayer)
    {
        if (x < 0 || x >= m_OccupationData.GetLength(0))
            return false;

        if (y < 0 || y >= m_OccupationData.GetLength(1))
            return false;

        if (isPlayer && y >= m_OccupationData.GetLength(1) / 2)
            return false;

        if (!isPlayer && y < m_OccupationData.GetLength(1) / 2)
            return false;

        return !m_OccupationData[x, y];
    }
}

public abstract class BoardElement : MonoBehaviour
{
    public enum ELEMENT_TYPE
    {
        UNDEFINED,
        PLAYER, 
        ENEMY,
        OBSTACLE
    }

    public abstract void Initialize(RoomEntity entity);
    public abstract void SetPosition(int x, int y);
    public abstract Vector2Int GetPosition();
    public abstract ELEMENT_TYPE GetElementType();
    public abstract void ApplyDamage(int damage);
}
