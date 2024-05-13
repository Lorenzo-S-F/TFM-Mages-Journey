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
        RoomManager.ValidEncounter encounter = room.GetEncounter(node.m_Rarity);
        SetupRoom(room, player, encounter);
        SetupEncounter(encounter);
    }

    private void SetupEncounter(RoomManager.ValidEncounter encounter)
    {
        //throw new NotImplementedException();
    }

    public void SetupRoom(RoomManager roomManager, RoomEntity player, RoomManager.ValidEncounter encounter)
    {
        m_CurrentRoom = Instantiate(roomManager);
        player.m_Position = encounter.m_PlayerInitialState;

        GameObject playerObject = Instantiate(player.m_EntityGameObject, m_CurrentRoom.GetPlayerTransform());
        playerObject.transform.SetParent(m_CurrentRoom.GetPlayerTransform());

        m_PlayerBase.InitializePlayer(playerObject.transform, player);
        m_PlayerController.SetPlayer(m_PlayerBase);
        SetBoardElementPosition(m_PlayerBase, player.m_Position.x, player.m_Position.y);
    }

    private IEnumerator DelayedSetup(RoomManager room)
    {
        yield return null;
    }

    public void InitializeBoardElement(BoardElement element, int x, int y)
    {
        element.Initialize();
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

    public abstract void Initialize();
    public abstract void SetPosition(int x, int y);
    public abstract ELEMENT_TYPE GetElementType();
}
