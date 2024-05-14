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
