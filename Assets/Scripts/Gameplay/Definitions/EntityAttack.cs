using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack", menuName = "GameAssets/AttackDef", order = 2)]
public class EntityAttack : ScriptableObject
{
    public int m_BaseDamage;
    public int m_BaseAttackSpeed;
    public int m_BaseShotSpeed;
    public AttackType m_AttackType;
}
