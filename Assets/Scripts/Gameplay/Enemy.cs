using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

public class Enemy : BoardElement
{
    private RoomEntity m_CurrentRoomEntity;
    private float m_ActionCooldown = 0;
    private GameManager m_GameManager;
    private RoomEntity m_PlayerEntity;
    private bool m_Moving = false;
    private bool m_Initialized = false;

    private bool m_Dashing = false;
    private Coroutine m_DashCoroutine;

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
        if (m_Dashing)
            return;

        if (m_ActionCooldown > 0)
        {
            m_ActionCooldown -= Time.deltaTime;
            return;
        }

        if ((m_PlayerEntity.m_Position.x == m_CurrentRoomEntity.m_Position.x || m_PlayerEntity.m_Position.y == m_CurrentRoomEntity.m_Position.y))
        {
            Vector2Int dir = m_GameManager.GetPlayerDirection(this);
            if (!m_GameManager.IsAnyObstacle(Mathf.Abs(m_PlayerEntity.m_Position.y - m_CurrentRoomEntity.m_Position.y) - 1, dir, m_CurrentRoomEntity.m_Position))
            {
                Shoot(dir);
                m_ActionCooldown = m_CurrentRoomEntity.m_Entity.m_AttackRate;
            }
        }
        else
        {
            int idleRate = (int)(m_CurrentRoomEntity.m_Entity.m_IdleRate * 100);
            int moveRate = (int)(m_CurrentRoomEntity.m_Entity.m_MoveRate * 100);
            int total = moveRate + idleRate;

            int value = RandomNumberGenerator.GetInt32(0, total);

            if (value > idleRate)
            {
                int dir = RandomNumberGenerator.GetInt32(0, 4);
                switch (dir)
                {
                    case 0:
                        Dash(new Vector2Int(1, 0));
                        break;
                    case 1:
                        Dash(new Vector2Int(-1, 0));
                        break;
                    case 2:
                        Dash(new Vector2Int(0, 1));
                        break;
                    case 3:
                        Dash(new Vector2Int(0, -1));
                        break;
                }
                m_ActionCooldown = m_CurrentRoomEntity.m_Entity.m_IdleTimeAfterMove;
            }
            else
                m_ActionCooldown = m_CurrentRoomEntity.m_Entity.m_IdleTime;
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

        int selectedShot = RandomNumberGenerator.GetInt32(0, m_CurrentRoomEntity.m_Entity.m_AttackData.Count);

        GameObject attack = Instantiate(m_CurrentRoomEntity.m_Entity.m_AttackData[selectedShot].m_Projectile, GameplayManagers.Instance.m_GameManager.m_ProjectilesTransform);
        attack.transform.localPosition = transform.position;

        var bullets = attack.GetComponentsInChildren<Attack>().ToList();
        var extraAttack = attack.GetComponent<Attack>();
        if (extraAttack != null)
            bullets.Add(extraAttack);

        for (int i = 0; i < bullets.Count; ++i)
            bullets[i].Initialize(m_CurrentRoomEntity.m_Entity.m_AttackData[selectedShot], dir, m_CurrentRoomEntity.m_Entity.m_EntityStats, "Enemy");

        if (dir.x != 0)
            attack.gameObject.transform.Rotate(new Vector3(0, 0, 90));
    }

    public void Dash(Vector2Int _direction)
    {
        Vector2Int expectedDirection = m_CurrentRoomEntity.m_Position + _direction;
        if (m_GameManager.IsValidPosition(expectedDirection.x, expectedDirection.y))
        {
            m_DashCoroutine = StartCoroutine(DashCoroutine(_direction));
        }
    }

    private IEnumerator DashCoroutine(Vector2Int _direction)
    {
        if (m_Dashing)
            yield break;

        m_CurrentRoomEntity.m_Position += _direction;
        m_Dashing = true;

        Vector3 startPosition = transform.localPosition;
        float t = 0;
        float smoothValue;

        while (t < 1)
        {
            yield return null;

            t += Time.deltaTime * m_CurrentRoomEntity.m_Entity.m_EntityStats.m_Speed;

            smoothValue = Mathf.SmoothStep(0, 1, t);
            transform.localPosition = startPosition + new Vector3(_direction.x * smoothValue, _direction.y * smoothValue, 0);
        }

        transform.localPosition = startPosition + new Vector3(_direction.x, _direction.y, 0);

        m_DashCoroutine = null;
        m_Dashing = false;
    }
}
