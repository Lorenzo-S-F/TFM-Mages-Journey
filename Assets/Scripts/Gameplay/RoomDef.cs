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
}

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
    public int m_HP;
    public int m_Damage;
    public int m_Speed;
    public int m_ShotSpeed;
    public int m_AttackSpeed;
}

[CreateAssetMenu(fileName = "Attack", menuName = "GameAssets/AttackDef", order = 2)]
public class EntityAttack : ScriptableObject
{
    public int m_BaseDamage;
    public int m_BaseAttackSpeed;
    public int m_BaseShotSpeed;
    public AttackType m_AttackType;
}

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

#region UNUSED
/*public class GameGrid<T>
{
    public int m_SizeX;
    public int m_SizeY;
    public T[,] m_Content;

    public void UpdateContent()
    {
        T[,] currentContent = m_Content;

        m_Content = new T[m_SizeX, m_SizeY];

        if (currentContent == null)
            return;
        
        for (int i = 0; i < currentContent.GetLength(0); ++i)
        {
            for (int j = 0; j < currentContent.GetLength(1); ++j)
            {
                if (i >= m_Content.GetLength(0) || j >= m_Content.GetLength(1))
                    continue;

                m_Content[i, j] = currentContent[i, j];
            }
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(GameGrid<BoardEntity>))]
public class GridEditor : Editor
{
    SerializedProperty grid;

    private void OnEnable()
    {
        grid = serializedObject.FindProperty("grid");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(grid);
        serializedObject.ApplyModifiedProperties();
    }

    /*
    public override void OnInspectorGUI()
    {
        GameGrid<BoardEntity> grid = target;

        EditorGUILayout.BeginVertical();

        GUILayout.Label("WARNING: Save and commit your prefab/scene OFTEN!");

        for (int y = 0; y < grid.down; y++)
        {
            GUILayout.BeginHorizontal();
            for (int x = 0; x < grid.across; x++)
            {
                int n = grid.GetIndex(x, y);

                var cell = grid.data.Substring(n, 1);

                // hard-coded some cheesy color map - improve it by all means!
                GUI.color = Color.gray;
                if (cell == "1") GUI.color = Color.white;
                if (cell == "2") GUI.color = Color.red;
                if (cell == "3") GUI.color = Color.green;

                if (GUILayout.Button(cell, GUILayout.Width(20)))
                {
                    grid.ToggleCell(x, y);
                }
            }
            GUILayout.EndHorizontal();
        }
        GUI.color = Color.yellow;

        GUILayout.Label("DANGER ZONE BELOW THIS AREA!");

        GUI.color = Color.white;

        EditorGUILayout.EndVertical();

        DrawDefaultInspector();
    }
    */
//}*/
//#endif
#endregion
