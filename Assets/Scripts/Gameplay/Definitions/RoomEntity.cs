using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public class RoomEntity
{
    public GameObject m_EntityGameObject;

    public Vector2Int m_Position;
    public Entity m_Entity;
    public ENTITY_TYPE m_Type;

    public enum ENTITY_TYPE
    {
        UNDEFINED,
        OBSTACLE,
        ENEMY,
        NPC,
        PLAYER
    }

    public RoomEntity Duplicate()
    {
        RoomEntity roomEnt = new RoomEntity();
        roomEnt.m_Entity = ScriptableObject.Instantiate(m_Entity);
        roomEnt.m_EntityGameObject = m_EntityGameObject;
        roomEnt.m_Position = m_Position;
        roomEnt.m_Type = m_Type;
        return roomEnt;
    }
}
