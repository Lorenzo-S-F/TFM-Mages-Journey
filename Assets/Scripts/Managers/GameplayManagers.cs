using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameplayManagers : Singleton<GameplayManagers>
{
    public Camera m_GameplayCamera;
    public GameManager m_GameManager;
    public LevelManager m_LevelManager;
    public RoomSelectionManager m_SelectionManager;

    public CommonButtonHandler m_AcceptButton;

    public IEnumerator Initialize()
    {
        m_LevelManager.GenerateLevel();
        (RoomManager room, LevelManager.MapNode node) = m_LevelManager.GetRoom(0);

        RoomEntity player = m_LevelManager.GetPlayerBase(0).Duplicate();

        m_GameManager.InitializeGame(room, node, player);
        yield return null;
    }

    public void LoadNextRoomCall(int roomIndex)
    {
        StartCoroutine(LoadNextRoom(roomIndex));
    }

    private IEnumerator LoadNextRoom(int roomIndex)
    {
        yield return m_SelectionManager.HideSelection();

        (RoomManager room, LevelManager.MapNode node) = m_LevelManager.GetRoom(roomIndex);
        m_GameManager.InitializeGame(room, node, m_GameManager.GetPlayerEntity());
        yield return null;
    }

    public IEnumerator ShowNextRoomOptions()
    {
        yield return m_SelectionManager.SetupSelection(m_LevelManager.GetNextLayerMapNodes());
    }

    public void ShowAcceptButton(UnityAction onClick)
    {
        m_AcceptButton.gameObject.SetActive(true);
        m_AcceptButton.m_OnClick.RemoveAllListeners();
        m_AcceptButton.m_OnClick.AddListener(onClick);
        m_AcceptButton.m_OnClick.AddListener(() => m_AcceptButton.gameObject.SetActive(false));
    }

    public void HideAcceptButton()
    {
        m_AcceptButton.gameObject.SetActive(false);
        m_AcceptButton.m_OnClick.RemoveAllListeners();
    }
}
