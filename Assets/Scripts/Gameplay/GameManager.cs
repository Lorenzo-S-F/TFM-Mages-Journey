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
    private Coroutine m_CurrentSlowTime = null;
    private GameplayManagers m_GameplayManagers;

    [SerializeField]
    private float m_LerpSlowTime = 0.1f;
    [SerializeField]
    private float m_ExtraSlowTime = 0.1f;
    [SerializeField]
    private float m_SlowTimeMultiplier = 0.5f;

    private void Awake()
    {
        m_GameplayManagers = GameplayManagers.Instance;
    }

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

    internal int GetPlayerGold()
    {
        return m_PlayerBase.GetCurrentGold();
    }

    internal void AddGoldToPlayer(int gold)
    {
        m_PlayerBase.AddGold(gold);
    }

    public void ApplyItemToPlayer(Item item)
    {
        m_PlayerBase.ApplyItem(item);
    }

    public bool IsAnyObstacle(Vector2Int direction, Vector2Int startPos, Vector2Int endPos)
    {
        startPos += direction;
        while(endPos.x != startPos.x || endPos.y != startPos.y)
        {
            if (startPos.x < 0 || startPos.x >= m_BoardSizeX || startPos.y < 0 || startPos.y >= m_BoardSizeY)
                return false;

            if (m_OccupationData[startPos.x, startPos.y])
                return true;
            startPos += direction;
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

        SetupRoom(room, player, node.m_Rarity);
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
        bool selectedIdBlocked = false;
        int targetValue = 0;

        foreach(var entity in m_BoardElements)
        {
            int currentTargetValue = 0;

            if (entity == null || entity.GetElementType() == BoardElement.ELEMENT_TYPE.PLAYER)
                continue;

            if (selectedIsAligned && entity.GetPosition().x != element.GetPosition().x && entity.GetPosition().y != element.GetPosition().y)
                continue;

            bool entityAligned = entity.GetPosition().x == element.GetPosition().x || entity.GetPosition().y == element.GetPosition().y;

            float dist = Vector2.Distance(entity.GetPosition(), element.GetPosition());

            bool blocked = true;
            
            if (entityAligned)
            {
                currentTargetValue += 1;
                Vector2 direct = new Vector2Int(entity.GetPosition().x - element.GetPosition().x, entity.GetPosition().y - element.GetPosition().y);
                direct = direct.normalized;
                Vector2Int direction = new Vector2Int((int)direct.x, (int)direct.y);
                blocked = IsAnyObstacle(direction, element.GetPosition(), entity.GetPosition());
                if (!blocked)
                    currentTargetValue += 2;
            }

            if (currentTargetValue > targetValue || (currentTargetValue == targetValue && dist < minDist))
            {
                selectedIsAligned = entityAligned;
                closestElement = entity;
                minDist = dist;
                selectedIdBlocked = blocked;
                targetValue = currentTargetValue;
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

    internal void SlowTime()
    {
        if (m_CurrentSlowTime == null)
            m_CurrentSlowTime = StartCoroutine(SlowTimeCoroutine());
    }

    private IEnumerator SlowTimeCoroutine()
    {
        float t = 0;
        while (t < m_LerpSlowTime)
        {
            t += Time.deltaTime;
            m_GameplayManagers.m_TimeMultiplier = Mathf.Lerp(1, m_SlowTimeMultiplier, t / m_LerpSlowTime);
            yield return null;
        }

        yield return new WaitForSeconds(m_ExtraSlowTime);

        m_GameplayManagers.m_TimeMultiplier = 1;
        m_CurrentSlowTime = null;
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

    public void SetupRoom(RoomManager roomManager, RoomEntity player, LevelManager.MapNode.RARITY encounterRarity)
    {
        if (m_CurrentRoom != null)
        {
            Destroy(m_CurrentRoom.gameObject);
            m_CurrentRoom = null;
        }

        m_CurrentRoom = Instantiate(roomManager, m_BoardTransform);

        m_CurrentRoom.Initialize();
        RoomManager.ValidEncounter encounter = m_CurrentRoom.GetEncounter(encounterRarity);

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
    public abstract void ApplyDamage(float damage);
}
