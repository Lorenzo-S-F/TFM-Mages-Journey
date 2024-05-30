using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EntityAttack
{
    public int m_BaseDamage;
    public int m_BaseAttackSpeed;
    public int m_BaseShotSpeed;
    public AttackType m_AttackType;
    public GameObject m_Projectile;
}
