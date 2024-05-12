using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Player m_PlayerReference;
    [SerializeField]
    private List<BoardElement> m_BoardElements;

    [SerializeField]
    private Vector2Int m_PlayerStartPos;
    private bool [,] m_OccupationData;

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

    internal void InitializeGame(RoomDef room)
    {
        Setup(room);
    }

    public void Setup(RoomDef boardDefinition)
    {
        InitializeBoardElement(m_PlayerReference, m_PlayerStartPos.x, m_PlayerStartPos.y);
        m_OccupationData = new bool[m_BoardSizeX, m_BoardSizeY];

        foreach (var entity in boardDefinition.m_BoardEntities)
        {
            if (entity.m_Type != RoomEntity.ENTITY_TYPE.OBSTACLE)
                continue;

            m_OccupationData[entity.m_Position.x, entity.m_Position.y] = true;
        }
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
