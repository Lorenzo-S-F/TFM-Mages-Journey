using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManagers : Singleton<MainManagers>
{
    public LoadingHandler m_LoadingHandler;
    public AudioManager m_AudioManager;

    public int m_SelectedPlayerCharacter = 0;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    public void SetSelectedCharacter(int value)
    {
        m_SelectedPlayerCharacter = value;
    }
}
