using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotHandler : MonoBehaviour
{
    public TMPro.TextMeshProUGUI m_DescText;
    public SpriteRenderer m_ItemSprite;
    public Item m_ContainedItem;
    private GameplayManagers m_GameplayManager;
    [SerializeField] bool m_IsShop = false;
    private int m_Price = 5;
    public Collider2D m_ItemCollider;

    private void Awake()
    {
        m_GameplayManager = GameplayManagers.Instance;
    }

    public void SetItem(Item item)
    {
        m_ItemSprite.sprite = item.m_ItemSprite;
        if (m_IsShop)
            m_DescText.text = $"{m_Price}$";
        else
            m_DescText.text = string.Empty;
        m_ContainedItem = item;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            GameplayManagers.Instance.ShowDescriptionText(m_ContainedItem.m_Text);

            if (m_IsShop && GameplayManagers.Instance.m_GameManager.GetPlayerGold() >= m_Price)
            {
                GameplayManagers.Instance.ShowBuyButton(
                    () =>
                    {
                        m_GameplayManager.m_GameManager.ApplyItemToPlayer(m_ContainedItem);
                        m_GameplayManager.m_LevelManager.RemoveItemFormPool(m_ContainedItem);
                        m_GameplayManager.m_GameManager.AddGoldToPlayer(-m_Price);
                        GameplayManagers.Instance.HideAcceptButton();
                        m_ItemCollider.enabled = false;
                        m_ItemSprite.sprite = null;
                    }
                );
                GameplayManagers.Instance.HideExitButton();
            }
            else if (!m_IsShop)
            {
                GameplayManagers.Instance.ShowAcceptButton
                (
                    () =>
                    {
                        m_GameplayManager.m_GameManager.ApplyItemToPlayer(m_ContainedItem);
                        m_GameplayManager.m_LevelManager.RemoveItemFormPool(m_ContainedItem);
                        StartCoroutine(m_GameplayManager.ShowNextRoomOptions());
                        m_ItemCollider.enabled = false;
                        m_ItemSprite.sprite = null;
                        m_DescText.text = string.Empty;
                    }
                );
                GameplayManagers.Instance.HideExitButton();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            GameplayManagers.Instance.HideDecriptionText();

            if (m_IsShop)
            {
                GameplayManagers.Instance.HideBuyButton();
                GameplayManagers.Instance.ShowExitButton(
                    () =>
                    {
                        StartCoroutine(m_GameplayManager.ShowNextRoomOptions());
                    }
                );
            }
            else
            {
                GameplayManagers.Instance.HideAcceptButton();
                GameplayManagers.Instance.ShowExitButton(
                    () =>
                    {
                        StartCoroutine(m_GameplayManager.ShowNextRoomOptions());
                    }
                );
            }
        }
    }

}
