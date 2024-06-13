using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecialAttackHandler : MonoBehaviour
{
    [SerializeField]
    private Image m_AttackImage;
    [SerializeField]
    private TMPro.TextMeshProUGUI m_CountDownText;
    [SerializeField]
    private Button m_AttackButton;

    private float m_AttackCooldown;
    private EntityAttack m_EntityAttack;
    private GameplayManagers m_GameplayManagers;
    private bool m_InCooldown = false;
    private Player m_PlayerReference;

    private void Awake()
    {
        m_GameplayManagers = GameplayManagers.Instance;
    }

    public void Initialize(Sprite attackImage, EntityAttack attack, Player player)
    {
        m_AttackImage.sprite = attackImage;
        m_PlayerReference = player;
        m_EntityAttack = attack;

        m_AttackButton.interactable = true;
        m_CountDownText.text = string.Empty;
        m_AttackImage.enabled = true;
        m_InCooldown = false;
        m_AttackCooldown = 0;
    }

    private void Update()
    {
        if (m_AttackCooldown > 0)
        {
            m_AttackCooldown -= (m_GameplayManagers.m_TimeMultiplier * Time.deltaTime);
            m_CountDownText.text = (Mathf.Ceil(m_AttackCooldown)).ToString();
        }
        else if (m_InCooldown)
        {
            m_AttackButton.interactable = true;
            m_CountDownText.text = string.Empty;
            m_AttackImage.enabled = true;
            m_InCooldown = false;
        }
    }

    public EntityAttack GetSpecialAttack()
    {
        return m_EntityAttack;
    }

    public void OnSpecialAttackClicked()
    {
        if (m_InCooldown)
            return;

        if (m_PlayerReference.IsDashing() && !m_PlayerReference.IsPerfectDashing())
            return;

        m_PlayerReference.ShootSpecialAttack();
        m_AttackCooldown = 10;
        m_InCooldown = true;

        m_CountDownText.text = 10.ToString();
        m_AttackButton.interactable = false;
        m_AttackImage.enabled = false;
    }

}
