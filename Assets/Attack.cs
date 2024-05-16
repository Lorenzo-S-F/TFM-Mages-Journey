using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    private EntityAttack m_Attack;
    private EntityStats m_EntityStats;
    private Vector3 m_Direction;
    private float m_MaxTimeInScreen = 10f;
    private float m_CurrentTimeInScreen = 0f;
    private string m_SourceTag;

    public void Initialize(EntityAttack attack, Vector2 direction, EntityStats source, string sourceTag)
    {
        m_Attack = attack;
        m_Direction = new Vector3(direction.x, direction.y, 0);
        m_EntityStats = source;
        m_SourceTag = sourceTag;
    }

    private void Update()
    {
        if (m_MaxTimeInScreen > m_CurrentTimeInScreen)
        {
            transform.position += (m_Direction * m_Attack.m_BaseShotSpeed * m_EntityStats.m_ShotSpeed * Time.deltaTime);
            m_CurrentTimeInScreen += Time.deltaTime;
        }
        else
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != m_SourceTag)
        {
            if (collision.tag == "Enemy")
            {
                collision.gameObject.GetComponent<BoardElement>().ApplyDamage(m_Attack.m_BaseDamage * m_EntityStats.m_Damage);

                if (m_Attack.m_AttackType.m_DamageEffect == AttackType.DAMAGE_EFFECT.SINGLE)
                    Destroy(gameObject);
            }
            else if (collision.tag == "Player")
            {
                GameplayManagers.Instance.m_GameManager.m_PlayerBase.ApplyDamage(m_Attack.m_BaseDamage * m_EntityStats.m_Damage);

                if (m_Attack.m_AttackType.m_DamageEffect == AttackType.DAMAGE_EFFECT.SINGLE)
                    Destroy(gameObject);
            }
        }
    }

}
