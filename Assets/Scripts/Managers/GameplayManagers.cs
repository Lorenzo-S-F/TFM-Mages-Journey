using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManagers : Singleton<GameplayManagers>
{
    public Camera m_GameplayCamera;
    public GameManager m_GameManager;
    public LevelManager m_LevelManager;

    public IEnumerator Initialize()
    {
        m_LevelManager.GenerateLevel();
        (RoomManager room, LevelManager.MapNode node) = m_LevelManager.GetRoom(0);

        RoomEntity player = m_LevelManager.GetPlayerBase(0).Duplicate();

        m_GameManager.InitializeGame(room, node, player);
        yield return null;
    }

    public IEnumerator LoadNextRoom()
    {
        (RoomManager room, LevelManager.MapNode node) = m_LevelManager.GetRoom(0);
        m_GameManager.InitializeGame(room, node, m_GameManager.GetPlayerEntity());
        yield return null;
    }
}
