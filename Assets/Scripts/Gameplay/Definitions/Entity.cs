using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Entity", menuName = "GameAssets/EntityDef", order = 1)]
public class Entity : ScriptableObject
{
    public List<EntityAttack> m_AttackData;
    public float m_MoveRate;
    public float m_AttackRate;
    public float m_IdleRate;
    public EntityStats m_EntityStats;
}

[Serializable]
public class EntityStats
{
    public float m_HP;
    public int m_Damage;
    public int m_Speed;
    public int m_ShotSpeed;
    public int m_AttackSpeed;
}
