using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupController : MonoBehaviour
{
    public PopupsManager.POPUPS m_PopupType = PopupsManager.POPUPS.UNDEFINED;
    public CanvasGroup m_PopupCanvas;
    public float m_AnimationSpeed = 1;
    public float m_ResizeScale = 1;

    public GameObject m_GameplayExtraButton;

    private void Awake()
    {
        m_PopupCanvas.alpha = 0;
        m_PopupCanvas.interactable = false;
        m_PopupCanvas.blocksRaycasts = false;
    }

    public void OnEntry()
    {
        GameplayManagers manager = GameplayManagers.Instance;
        if (manager != null)
        {
            if (m_GameplayExtraButton != null)
                m_GameplayExtraButton.SetActive(true);
            manager.m_TimeMultiplier = 0;
        }
        else
            m_GameplayExtraButton.SetActive(false);

        StartCoroutine(OnEntryAnimation());
    }

    public IEnumerator OnEntryAnimation()
    {
        m_PopupCanvas.alpha = 1;
        m_PopupCanvas.blocksRaycasts = true;

        yield return null;

        m_PopupCanvas.interactable = true;
        Canvas.ForceUpdateCanvases();

        float t = 0;
        Vector3 initScale = transform.localScale;
        Vector3 finalScales = transform.localScale * m_ResizeScale;
        while (t < 1f)
        {
            transform.localScale = Vector3.Lerp(initScale, finalScales, t);
            t += Time.deltaTime * m_AnimationSpeed;
            yield return null;
        }

        t = 0;
        while (t < 1f)
        {
            transform.localScale = Vector3.Lerp(finalScales, initScale, t);
            t += Time.deltaTime * m_AnimationSpeed;
            yield return null;
        }

        transform.localScale = initScale;
        m_PopupCanvas.interactable = true;
    }

    public void OnExitButtonClicked()
    {
        MainManagers.Instance.m_LoadingHandler.LoadScene(LoadingHandler.SCENE.MENUS);
        ClosePopup();
    }

    public void ClosePopup()
    {
        GameplayManagers manager = GameplayManagers.Instance;

        if (m_GameplayExtraButton != null)
            m_GameplayExtraButton.SetActive(true);

        if (manager != null)
            manager.m_TimeMultiplier = 1;

        m_PopupCanvas.blocksRaycasts = false;
        m_PopupCanvas.interactable = false;
        PopupsManager.Instance.ClosePopup();
    }
}
