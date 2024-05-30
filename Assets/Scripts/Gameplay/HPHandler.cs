using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPHandler : MonoBehaviour
{
    [SerializeField] private Sprite m_FilledHPSlot;
    [SerializeField] private Sprite m_EmptyHPSlot;
    [SerializeField] private Sprite m_FilledHPSlotEnd;
    [SerializeField] private Sprite m_EmptyHPSlotEnd;
    [SerializeField] private Image m_EmptyImage;
    [SerializeField] private ParticleSystem m_DropHpEffect;
    private List<Image> m_CurrentHPImages = new List<Image>();

    public void Initialize(int amount)
    {
        for (int i = 0; i < amount; ++i)
        {
            Image slot = Instantiate(m_EmptyImage, transform);
            if (i == amount-1)
                slot.sprite = m_FilledHPSlotEnd;
            else
                slot.sprite = m_FilledHPSlot;
            m_CurrentHPImages.Add(slot);
        }
    }

    public void ModifyCurrentHP(int currentHP)
    {
        int count = m_CurrentHPImages.Count - 1;
        for (int i = 0; i < m_CurrentHPImages.Count; ++i)
        {
            if (currentHP > 0)
            {
                if (i == count)
                    m_CurrentHPImages[i].sprite = m_FilledHPSlotEnd;
                else
                    m_CurrentHPImages[i].sprite = m_FilledHPSlot;

                currentHP--;
            }
            else
            {
                if (i == count)
                    m_CurrentHPImages[i].sprite = m_EmptyHPSlotEnd;
                else
                    m_CurrentHPImages[i].sprite = m_EmptyHPSlot;

                m_DropHpEffect.Play();
            }
        }
    }

    public void ModifyAmount(int currentAmount, int currentHp)
    {
        int difference = currentAmount - m_CurrentHPImages.Count;
        if (difference > 0)
        {
            for (int i = 0; i < difference; ++i)
            {
                Image slot = Instantiate(m_EmptyImage, transform);
                slot.sprite = m_FilledHPSlot;
                m_CurrentHPImages.Add(slot);
            }
        }
        else if (difference < 0)
        {
            for (int i = 0; i < difference; ++i)
            {
                Destroy(m_CurrentHPImages[0].gameObject);
                m_CurrentHPImages.RemoveAt(0);
            }
        }

        ModifyCurrentHP(currentHp);
    }
}
