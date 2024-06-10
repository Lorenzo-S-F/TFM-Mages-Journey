using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldHandler : MonoBehaviour
{
    public TMPro.TextMeshProUGUI m_GoldText;

    public void SetCurrentGold(int value)
    {
        m_GoldText.text = value.ToString();
    }

}
