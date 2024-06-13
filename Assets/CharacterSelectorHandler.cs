using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectorHandler : MonoBehaviour
{
    [SerializeField]
    private List<LevelManager.Pair<string, Sprite>> m_SelectorData;

    [SerializeField]
    private TMPro.TextMeshProUGUI m_CharacterDescription;

    [SerializeField]
    private Image m_CharacterImage;
    private int m_Current;

    private void Awake()
    {
        m_Current = PlayerPrefs.GetInt("SelectedCharacter", 0);
        SetView(m_Current);
    }

    private void SetView(int value)
    {
        m_CharacterImage.sprite = m_SelectorData[value].Value;
        m_CharacterDescription.text = m_SelectorData[value].Key;
        m_Current = value;
        MainManagers.Instance.m_SelectedPlayerCharacter = value;
    }

    public void ViewNext()
    {
        if (m_Current >= m_SelectorData.Count - 1)
            m_Current = 0;
        else
            m_Current++;

        SetView(m_Current);
    }

    public void ViewPrevious()
    {
        if (m_Current <= 0)
            m_Current = m_SelectorData.Count - 1;
        else
            m_Current--;

        SetView(m_Current);
    }

}
