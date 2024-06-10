using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupController : MonoBehaviour
{
    public PopupsManager.POPUPS m_PopupType = PopupsManager.POPUPS.UNDEFINED;
    public CanvasGroup m_PopupCanvas;
    public float m_AnimationSpeed = 1;
    public float m_ResizeScale = 1;

    private void Awake()
    {
        m_PopupCanvas.alpha = 0;
        m_PopupCanvas.interactable = false;
        m_PopupCanvas.blocksRaycasts = false;
    }

    public void OnEntry()
    {
        StartCoroutine(OnEntryAnimation());
    }

    public IEnumerator OnEntryAnimation()
    {
        m_PopupCanvas.alpha = 1;
        m_PopupCanvas.interactable = false;
        m_PopupCanvas.blocksRaycasts = true;

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

    public void ClosePopup()
    {
        m_PopupCanvas.blocksRaycasts = false;
        PopupsManager.Instance.ClosePopup();
    }
}
