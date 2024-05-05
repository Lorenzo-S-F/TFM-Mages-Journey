using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSizeHandler : MonoBehaviour
{
    [SerializeField]
    private Camera m_Camera;

    [SerializeField]
    private float m_DesiredRatioSize = 8;
    [SerializeField]
    private float m_DesiredAspectRatio = 0.5625f;

    // Start is called before the first frame update
    void Start()
    {
        SetupCameraSizeByBoardSize();
    }

    public void SetupCameraSizeByBoardSize()
    {
        float cameraRatio = m_Camera.aspect;

        m_Camera.orthographicSize = (m_DesiredAspectRatio * m_DesiredRatioSize) / cameraRatio;
        transform.position = new Vector3(0, 0, -1f);
    }
}
