using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpDownMove : MonoBehaviour
{
    [SerializeField]
    private Vector3 m_MaxPos;
    [SerializeField]
    private Vector3 m_MinPos;

    private bool m_GoingToMax = true;
    private float m_T;
    private GameplayManagers m_GameplayManager;

    private void Awake()
    {
        m_GameplayManager = GameplayManagers.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_GoingToMax)
        {
            m_T += (Time.deltaTime * m_GameplayManager.m_TimeMultiplier);
            transform.localPosition = Vector3.Lerp(m_MinPos, m_MaxPos, m_T);
            if (m_T >= 1)
            {
                m_T = 0;
                m_GoingToMax = false;
            }
        }
        else
        {
            m_T += (Time.deltaTime * m_GameplayManager.m_TimeMultiplier);
            transform.localPosition = Vector3.Lerp(m_MaxPos, m_MinPos, m_T);
            if (m_T >= 1)
            {
                m_T = 0;
                m_GoingToMax = true;
            }
        }
    }
}
