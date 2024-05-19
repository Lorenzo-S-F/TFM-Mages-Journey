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

    private void Awake()
    {
        m_GameplayManager = GameplayManagers.Instance;
    }

    public void SetItem(Item item)
    {
        m_ItemSprite.sprite = item.m_ItemSprite;
        m_DescText.text = item.m_Text;
        m_ContainedItem = item;
    }

    [Serializable]
    public class Item
    {
        public string m_Text;
        public Sprite m_ItemSprite;
        public EntityStats m_PlaneStatModifiers;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            GameplayManagers.Instance.ShowAcceptButton
            (
                () =>
                {
                    m_GameplayManager.m_GameManager.ApplyItemToPlayer(m_ContainedItem);
                    m_GameplayManager.m_LevelManager.RemoveItemFormPool(m_ContainedItem);
                    StartCoroutine(m_GameplayManager.ShowNextRoomOptions());
                }
            );
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            GameplayManagers.Instance.HideAcceptButton();
        }
    }

}
