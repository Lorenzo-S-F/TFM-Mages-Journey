using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectorHandler : MonoBehaviour
{
    [SerializeField]
    private List<MageData> m_MagesData;

    [SerializeField]
    private TMPro.TextMeshProUGUI m_CharacterDescription;

    [SerializeField]
    private Image m_CharacterImage;

    [SerializeField]
    private Transform m_SelectedIconTransform;
    [SerializeField]
    private Image m_SelectedIconObject;

    [SerializeField]
    private Sprite m_SelectedIconImage;
    [SerializeField]
    private Sprite m_NotSelectedIconImage;

    private List<Image> m_ImagesList = new List<Image>();

    [SerializeField]
    private TMPro.TextMeshProUGUI m_MageTitle;
    private int m_Current;

    [Serializable]
    public class MageData
    {
        public string m_Info;
        public string m_Name;
        public Sprite m_Image;
    }

    private void Awake()
    {
        m_Current = PlayerPrefs.GetInt("SelectedCharacter", 0);
        for (int i = 0; i < m_MagesData.Count; ++i)
        {
            m_ImagesList.Add(Instantiate(m_SelectedIconObject, m_SelectedIconTransform));
        }

        SetView(m_Current);
    }

    private void SetView(int value)
    {
        m_CharacterDescription.text = m_MagesData[value].m_Info;
        m_CharacterImage.sprite = m_MagesData[value].m_Image;
        m_MageTitle.text = m_MagesData[value].m_Name;

        m_ImagesList[value].sprite = m_SelectedIconImage;

        MainManagers.Instance.m_SelectedPlayerCharacter = value;
    }

    public void ViewNext()
    {
        m_ImagesList[m_Current].sprite = m_NotSelectedIconImage;

        if (m_Current >= m_MagesData.Count - 1)
            m_Current = 0;
        else
            m_Current++;

        SetView(m_Current);
    }

    public void ViewPrevious()
    {
        m_ImagesList[m_Current].sprite = m_NotSelectedIconImage;

        if (m_Current <= 0)
            m_Current = m_MagesData.Count - 1;
        else
            m_Current--;

        SetView(m_Current);
    }

    public void PlayRandom()
    {
        SetView(RandomNumberGenerator.GetInt32(0, m_MagesData.Count));
        MainManagers.Instance.m_LoadingHandler.LoadScene(LoadingHandler.SCENE.GAMEPLAY);
    }

}
