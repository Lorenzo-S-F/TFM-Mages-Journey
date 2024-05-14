using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackPattern", menuName = "GameAssets/AttackPatternDef", order = 3)]
public class AttackType : ScriptableObject
{
    public ATTACK_TYPE m_AttackType;
    public THROW_TYPE m_ThrowType;
    public DAMAGE_EFFECT m_DamageEffect;
    public GameObject m_Projectile = null;
    public float m_DamageTime = 0;
    public float m_IntervalEffectTime = 0;

    public int m_SizeX;
    public int m_Sizey;

    public enum ATTACK_TYPE
    {
        UNDEFINED,
        AREA,
        ROW,
        COLUMN
    }

    public enum THROW_TYPE
    {
        UNDEFINED,
        GROUND,
        AIR
    }

    public enum DAMAGE_EFFECT
    {
        UNDEFINED,
        SINGLE,
        MULTIPLE,
        DOT
    }
}
