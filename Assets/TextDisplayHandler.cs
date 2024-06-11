using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextDisplayHandler : MonoBehaviour
{
    [SerializeField]
    private TMPro.TextMeshProUGUI m_DescriptionText;

    public void SetText(string text)
    {
        m_DescriptionText.text = text;
    }
}
