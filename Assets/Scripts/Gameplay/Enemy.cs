using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : BoardElement
{
    private RoomEntity m_CurrentRoomEntity;
    private float m_AttackCooldown = 0;
    private GameManager m_GameManager;
    private RoomEntity m_PlayerEntity;
    private bool m_Moving = false;
    private bool m_Initialized = false;

    private void Awake()
    {
        m_GameManager = GameplayManagers.Instance.m_GameManager;
        m_PlayerEntity = m_GameManager.GetPlayerEntity();
    }

    public override void ApplyDamage(int damage)
    {
        m_CurrentRoomEntity.m_Entity.m_EntityStats.m_HP -= damage;
        if (m_CurrentRoomEntity.m_Entity.m_EntityStats.m_HP <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (m_AttackCooldown > 0)
        {
            m_AttackCooldown -= Time.deltaTime;
            return;
        }

        if ((m_PlayerEntity.m_Position.x == m_CurrentRoomEntity.m_Position.x || m_PlayerEntity.m_Position.y == m_CurrentRoomEntity.m_Position.y))
        {
            Vector2Int dir = m_GameManager.GetPlayerDirection(this);
            if (!m_GameManager.IsAnyObstacle(Mathf.Abs(m_PlayerEntity.m_Position.y - m_CurrentRoomEntity.m_Position.y) - 1, dir, m_CurrentRoomEntity.m_Position))
            {
                Shoot(dir);
                m_AttackCooldown = m_CurrentRoomEntity.m_Entity.m_AttackRate;
            }
        }

    }

    public override ELEMENT_TYPE GetElementType()
    {
        return ELEMENT_TYPE.ENEMY;
    }

    public override Vector2Int GetPosition()
    {
        return m_CurrentRoomEntity.m_Position;
    }

    public override void Initialize(RoomEntity entity)
    {
        if (m_Initialized)
            return;

        m_Initialized = true;
        m_CurrentRoomEntity = entity.Duplicate();
    }

    public override void SetPosition(int x, int y)
    {
        transform.localPosition = new Vector3(x, y, 0);
    }

    public void Shoot(Vector2Int dir)
    {
        if (m_Moving || m_CurrentRoomEntity == null)
            return;

        GameObject attack = Instantiate(m_CurrentRoomEntity.m_Entity.m_AttackData[0].m_AttackType.m_Projectile, GameplayManagers.Instance.m_GameManager.m_ProjectilesTransform);
        attack.transform.localPosition = transform.position;
        attack.GetComponent<Attack>().Initialize(m_CurrentRoomEntity.m_Entity.m_AttackData[0], dir, m_CurrentRoomEntity.m_Entity.m_EntityStats, "Enemy");
    }
}
