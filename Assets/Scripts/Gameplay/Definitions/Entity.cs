using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Entity", menuName = "GameAssets/EntityDef", order = 1)]
public class Entity : ScriptableObject
{
    public List<EntityAttack> m_AttackData;
    public EntityStats m_EntityStats;

    public float m_IdleTimeAfterMove;
    public float m_AttackRate;
    public float m_MoveRate;
    public float m_IdleRate;
    public float m_IdleTime;
    public int m_GoldOnDeath = 1;

    public List<Modification> m_AttackModifications;

    [Serializable]
    public class Modification
    {
        public CHECK_TIME m_CheckTime;
        public MODIFICATION m_Modification;

        public enum CHECK_TIME
        {
            UNDEFINED,
            UPDATE,
            HIT
        }

        public enum MODIFICATION
        {
            UNDEFINED,
            DECREASE_ON_HIT,
            SIZE_BEAT,
            OBSTACLE_INMUNE,
            ENEMY_INMUNE
        }
    }
}

[Serializable]
public class EntityStats
{
    public float m_AttackProjectileTime = 4;
    public float m_AttackSpeed;
    public float m_ShotSpeed;
    public float m_Damage;
    public float m_Speed;
    public int m_HP;

    public EntityStats GetCopy()
    {
        EntityStats copy = new EntityStats()
        {
            m_AttackProjectileTime = m_AttackProjectileTime,
            m_AttackSpeed = m_AttackSpeed,
            m_ShotSpeed = m_ShotSpeed,
            m_Damage = m_Damage,
            m_Speed = m_Speed,
            m_HP = m_HP            
        };

        return copy;
    }

}
