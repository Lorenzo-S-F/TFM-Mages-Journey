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

    [SerializeField]
    private Player m_PlayerBase;
    private RoomManager m_CurrentRoom;

    internal void InitializeGame(RoomManager room, LevelManager.MapNode node, RoomEntity player)
    {
        m_BoardSizeX = room.GetRoomSize().x;
        m_BoardSizeY = room.GetRoomSize().y;

        m_OccupationData = new bool[m_BoardSizeX, m_BoardSizeY];

        RoomManager.ValidEncounter encounter = room.GetEncounter(node.m_Rarity);
        SetupRoom(room, player, encounter);
        SetupEncounter(encounter);
    }

    public bool IsValidPosition(int x, int y)
    {
        if (x < 0 || x >= m_BoardSizeX || y < 0 || y >= m_BoardSizeY || m_OccupationData[x, y] || m_BoardElements.FindIndex(element => (element.GetPosition().x == x && element.GetPosition().y == y)) != -1)
            return false;

        return true;
    }

    private void SetupEncounter(RoomManager.ValidEncounter encounter)
    {
        //throw new NotImplementedException();
    }

    public void SetupRoom(RoomManager roomManager, RoomEntity player, RoomManager.ValidEncounter encounter)
    {
        m_CurrentRoom = Instantiate(roomManager);

        GameObject playerObject = Instantiate(player.m_EntityGameObject, m_CurrentRoom.GetPlayerTransform());
        playerObject.transform.SetParent(m_CurrentRoom.GetPlayerTransform());

        m_PlayerBase.InitializeTransform(playerObject.transform);
        m_PlayerController.SetPlayer(m_PlayerBase);
        InitializeBoardElement(m_PlayerBase, encounter.m_PlayerInitialState.x, encounter.m_PlayerInitialState.y, player);

        foreach(var entity in encounter.m_RoomEnemies)
        {
            Enemy enemy = Instantiate(entity.m_EnemyData.m_EntityGameObject, m_CurrentRoom.GetGridTransform()).GetComponent<Enemy>();
            InitializeBoardElement(enemy, entity.m_StartPosition.x, entity.m_StartPosition.y, entity.m_EnemyData);
        }
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
}
