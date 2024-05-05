using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Player m_PlayerReference;
    [SerializeField]
    private Vector2Int m_PlayerStartPos;
    private bool [,] m_OccupationData;

    private int m_BoardSizeX;
    private int m_BoardSizeY;

    private void Start()
    {
        BoardDefinition hardCodedDef = new BoardDefinition();

        hardCodedDef.m_BoardElements = new BoardElement[9, 10];

        Setup(hardCodedDef);
    }

    public void Setup(BoardDefinition _boardDefinition)
    {
        int m_BoardSizeX = _boardDefinition.m_BoardElements.GetLength(0);
        int m_BoardSizeY = _boardDefinition.m_BoardElements.GetLength(1);

        InitializeBoardElement(m_PlayerReference, m_PlayerStartPos.x, m_PlayerStartPos.y);

        m_OccupationData = new bool[m_BoardSizeX, m_BoardSizeY];

        for (int i = 0; i < m_BoardSizeX; ++i)
            for (int j = 0; j < m_BoardSizeY; ++j)
                m_OccupationData[i, j] = _boardDefinition.m_BoardElements[i, j] != null;
    }

    public void InitializeBoardElement(BoardElement element, int x, int y)
    {
        element.Initialize();
        SetBoardElementPosition(element, x, y);
    }

    private void SetBoardElementPosition(BoardElement element, int x, int y)
    {
        element.SetPosition(x - (m_BoardSizeX - 1) / 2, y - (m_BoardSizeY - 2) / 2);
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

    public class BoardDefinition
    {
        public BoardElement[,] m_BoardElements;
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
