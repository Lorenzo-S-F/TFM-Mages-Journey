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

    // Update is called once per frame
    void Update()
    {
        if (m_GoingToMax)
        {
            m_T += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(m_MinPos, m_MaxPos, m_T);
            if (m_T >= 1)
            {
                m_T = 0;
                m_GoingToMax = false;
            }
        }
        else
        {
            m_T += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(m_MaxPos, m_MinPos, m_T);
            if (m_T >= 1)
            {
                m_T = 0;
                m_GoingToMax = true;
            }
        }
    }
}
