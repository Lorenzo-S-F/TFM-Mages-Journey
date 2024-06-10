using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EntityAttack
{
    public ShotSystem.Pattern m_AttackPattern;
    public GameObject m_Projectile;
    public THROW_TYPE m_ThrowType;
    public enum THROW_TYPE
    {
        UNDEFINED,
        GROUND,
        AIR
    }
}
