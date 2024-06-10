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
    public CommonButtonHandler m_BuyButton;
    public CommonButtonHandler m_ExitButton;
    public float m_TimeMultiplier = 1;

    public IEnumerator Initialize()
    {
        m_LevelManager.GenerateLevel();
        (RoomManager room, LevelManager.MapNode node) = m_LevelManager.GetRoom(0);

        LevelManager.PlayerPreset player = m_LevelManager.GetPlayerBase(0).Duplicate();

        m_GameManager.InitializeGame(room, node, player.m_PlayerEntity);

        foreach (var item in player.m_StartingItems)
        {
            m_GameManager.ApplyItemToPlayer(item);
            m_LevelManager.RemoveItemFormPool(item);
        }

        yield return null;
    }

    public void LoadNextRoomCall(int roomIndex)
    {
        StartCoroutine(LoadNextRoom(roomIndex));
    }

    private IEnumerator LoadNextRoom(int roomIndex)
    {
        RoomManager room = null; 
        LevelManager.MapNode node;
        yield return m_SelectionManager.HideSelection(
            () => 
            {
                (room, node) = m_LevelManager.GetRoom(roomIndex);
                m_GameManager.InitializeGame(room, node, m_GameManager.GetPlayerEntity());
            }
        );

        if (room.GetRoomType() == LevelManager.MapNode.ROOM_TYPE.SHOP || room.GetRoomType() == LevelManager.MapNode.ROOM_TYPE.ITEM)
            ShowExitButton(
                () => 
                {
                    StartCoroutine(ShowNextRoomOptions());
                });
    }

    public IEnumerator ShowNextRoomOptions()
    {
        HideAcceptButton();
        HideBuyButton();
        HideExitButton();
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


    public void ShowBuyButton(UnityAction onClick)
    {
        m_BuyButton.gameObject.SetActive(true);
        m_BuyButton.m_OnClick.RemoveAllListeners();
        m_BuyButton.m_OnClick.AddListener(onClick);
        m_BuyButton.m_OnClick.AddListener(() => m_AcceptButton.gameObject.SetActive(false));
    }

    public void HideBuyButton()
    {
        m_BuyButton.gameObject.SetActive(false);
        m_BuyButton.m_OnClick.RemoveAllListeners();
    }


    public void ShowExitButton(UnityAction onClick)
    {
        m_ExitButton.gameObject.SetActive(true);
        m_ExitButton.m_OnClick.RemoveAllListeners();
        m_ExitButton.m_OnClick.AddListener(onClick);
        m_ExitButton.m_OnClick.AddListener(() => m_AcceptButton.gameObject.SetActive(false));
    }

    public void HideExitButton()
    {
        m_ExitButton.gameObject.SetActive(false);
        m_ExitButton.m_OnClick.RemoveAllListeners();
    }

    public void OpenSettingsPopup()
    {
        PopupsManager.Instance.TryOpenPopup(PopupsManager.POPUPS.SETTINGS);
    }
}
