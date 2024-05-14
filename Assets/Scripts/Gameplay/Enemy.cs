using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : BoardElement
{
    private RoomEntity m_CurrentRoomEntity;

    public override ELEMENT_TYPE GetElementType()
    {
        return ELEMENT_TYPE.ENEMY;
    }

    public override Vector2Int GetPosition()
    {
        return m_CurrentRoomEntity.m_Position;
    }

    public override void Initialize(RoomEntity entity)
    {
        m_CurrentRoomEntity = entity;
    }

    public override void SetPosition(int x, int y)
    {
        transform.localPosition = new Vector3(x, y, 0);
    }
}
