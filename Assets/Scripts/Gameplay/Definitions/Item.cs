using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "GameAssets/ItemDef", order = 1)]
[Serializable]
public class Item : ScriptableObject
{
    public string m_Text;
    public Sprite m_ItemSprite;
    public EntityStats m_PlaneStatModifiers;
    public int m_HealAmount;
    public Entity.Modification m_Modification;
    public EntityAttack m_NewAttack;
    public EntityAttack m_NewHability;
    public List<ITEM_TYPE> m_ItemType;

    public enum ITEM_TYPE
    {
        STATS_MODIFIER_INCREASER,
        STATS_MODIFIER_MULTIPLIER,
        SHOT_MODIFIER,
        SHOT_SUBSTITUTE,
        HEAL,
        HABILITY
    }
}
