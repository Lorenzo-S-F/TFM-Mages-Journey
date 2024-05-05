using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeAreaHandler : MonoBehaviour
{
    [SerializeField]
    private RectTransform m_LocalRectTransform;
    [SerializeField]
    private Canvas m_CanvasRef;

    void Awake()
    {
        UpdateRect();
        m_LocalRectTransform.hasChanged = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_LocalRectTransform.hasChanged)
            UpdateRect();
    }

    private void UpdateRect()
    {
        Rect safeArea = Screen.safeArea;

        var anchorMin = safeArea.position;
        var anchorMax = safeArea.position + safeArea.size;
        anchorMin.x /= m_CanvasRef.pixelRect.width;
        anchorMin.y /= m_CanvasRef.pixelRect.height;
        anchorMax.x /= m_CanvasRef.pixelRect.width;
        anchorMax.y /= m_CanvasRef.pixelRect.height;

        m_LocalRectTransform.anchorMin = anchorMin;
        m_LocalRectTransform.anchorMax = anchorMax;

        m_LocalRectTransform.hasChanged = false;
        Debug.Log(safeArea);
    }
}
