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
        m_GameManager.InitializeGame(m_LevelManager.GetRoom(0));
        yield return null;
    }
}
