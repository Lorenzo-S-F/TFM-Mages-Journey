using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupsManager : Singleton<PopupsManager>
{
    [SerializeField] private List<LevelManager.Pair<POPUPS, PopupController>> m_AvaliableGameObjects;
    [SerializeField] private Image m_Fade;
    private PopupController m_ActivePopup;

    [Serializable]
    public enum POPUPS
    {
        UNDEFINED = 0,
        SETTINGS = 1
    }

    public void TryOpenButtonCall(int popup)
    {
        TryOpenPopup((POPUPS)popup);
    }

    public void TryOpenPopup(POPUPS popus)
    {
        int popupIndex = m_AvaliableGameObjects.FindIndex(x => x.Key == popus);
        if (popupIndex == -1)
        {
            Debug.LogError("Popup " + popus.ToString() + " not found");
            return;
        }

        if (!m_Fade.enabled)
            m_Fade.enabled = true;

        if (m_ActivePopup != null)
        {
            m_ActivePopup.m_PopupCanvas.alpha = 0;
            m_ActivePopup = null;
        }

        m_ActivePopup = m_AvaliableGameObjects[popupIndex].Value;
        m_ActivePopup.OnEntry();
    }

    public void ClosePopup()
    {
        if (m_ActivePopup == null)
            return;

        m_ActivePopup.m_PopupCanvas.alpha = 0;
        m_Fade.enabled = false;
        m_ActivePopup = null;
    }

}
