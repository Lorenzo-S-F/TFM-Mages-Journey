using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Entity", menuName = "GameAssets/EntityDef", order = 1)]
public class Entity : ScriptableObject
{
    public List<EntityAttack> m_AttackData;
    public float m_MoveRate;
    public float m_IdleTimeAfterMove;
    public float m_AttackRate;
    public float m_IdleRate;
    public float m_IdleTime;
    public EntityStats m_EntityStats;
}

[Serializable]
public class EntityStats
{
    public int m_HP;
    public int m_Damage;
    public float m_Speed;
    public float m_ShotSpeed;
    public float m_AttackSpeed;
}
