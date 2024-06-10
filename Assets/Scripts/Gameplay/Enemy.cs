using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

public class Enemy : BoardElement
{
    private GameManager m_GameManager;
    private RoomEntity m_CurrentRoomEntity;
    private RoomEntity m_PlayerEntity;

    private float m_ActionCooldown = 0;
    private bool m_Initialized = false;
    private bool m_Moving = false;

    private Material m_Material;

    private bool m_Dashing = false;
    private GameplayManagers m_GameplayManagers;

    private void Awake()
    {
#if UNITY_EDITOR
        m_Material = GetComponent<SpriteRenderer>().material;
#else
        m_Material = GetComponent<SpriteRenderer>().sharedMaterial;
#endif
        m_GameplayManagers = GameplayManagers.Instance;
        m_GameManager = GameplayManagers.Instance.m_GameManager;
        m_PlayerEntity = m_GameManager.GetPlayerEntity();
    }

    public override void ApplyDamage(float damage)
    {
        m_CurrentRoomEntity.m_Entity.m_EntityStats.m_HP -= Mathf.RoundToInt(damage);
        if (m_CurrentRoomEntity.m_Entity.m_EntityStats.m_HP <= 0)
        {
            GameplayManagers.Instance.m_GameManager.AddGoldToPlayer(m_CurrentRoomEntity.m_Entity.m_GoldOnDeath);
            Destroy(gameObject);
        }
        else
            StartCoroutine(ReceiveDamageMat());
    }

    private bool m_PlayerReachable = false;
    private void Update()
    {
        if (m_Dashing)
            return;

        if (m_ActionCooldown > 0)
        {
            m_ActionCooldown -= (Time.deltaTime * m_GameplayManagers.m_TimeMultiplier);
            return;
        }

        m_PlayerReachable = false;
        if ((m_PlayerEntity.m_Position.x == m_CurrentRoomEntity.m_Position.x || m_PlayerEntity.m_Position.y == m_CurrentRoomEntity.m_Position.y))
        {
            int selectedShot = RandomNumberGenerator.GetInt32(0, m_CurrentRoomEntity.m_Entity.m_AttackData.Count);

            Vector2Int dir = m_GameManager.GetPlayerDirection(this);
            if (!m_GameManager.IsAnyObstacle(dir, m_CurrentRoomEntity.m_Position, m_PlayerEntity.m_Position) 
                || m_CurrentRoomEntity.m_Entity.m_AttackData[selectedShot].m_ThrowType == EntityAttack.THROW_TYPE.AIR
                || m_CurrentRoomEntity.m_Entity.m_AttackModifications.FindIndex(z => z.m_Modification == Entity.Modification.MODIFICATION.OBSTACLE_INMUNE) != -1)
            {
                m_PlayerReachable = true;
                Shoot(dir, selectedShot);
                m_ActionCooldown = m_CurrentRoomEntity.m_Entity.m_AttackRate;
            }
        }

        if (!m_PlayerReachable)
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

    public void Shoot(Vector2Int dir, int selectedShot)
    {
        if (m_Moving || m_CurrentRoomEntity == null)
            return;

        StartCoroutine(ShotSystem.ShootPattern(
            m_CurrentRoomEntity.m_Entity.m_AttackData[selectedShot].m_AttackPattern,
            m_CurrentRoomEntity.m_Entity.m_AttackData[selectedShot].m_Projectile,
            this,
            dir,
            1,
            GameplayManagers.Instance.m_GameManager.m_ProjectilesTransform,
            (parent, projectile, distanceToEntity, bulletParent, rotatedDisplacement, direction) =>
            {
                GameObject attack = Instantiate(projectile, transform.position + rotatedDisplacement * distanceToEntity, Quaternion.identity, bulletParent);
                var bullets = attack.GetComponentsInChildren<Attack>().ToList();

                for (int i = 0; i < bullets.Count; ++i)
                    bullets[i].Initialize(m_CurrentRoomEntity.m_Entity.m_AttackData[selectedShot], direction, m_CurrentRoomEntity.m_Entity.m_EntityStats, "Enemy", m_CurrentRoomEntity.m_Entity.m_AttackModifications);
            }
            ));

    }

    public void Dash(Vector2Int _direction)
    {
        Vector2Int expectedDirection = m_CurrentRoomEntity.m_Position + _direction;
        if (m_GameManager.IsValidPosition(expectedDirection.x, expectedDirection.y))
            StartCoroutine(DashCoroutine(_direction));
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

            t += Time.deltaTime * m_CurrentRoomEntity.m_Entity.m_EntityStats.m_Speed * m_GameplayManagers.m_TimeMultiplier;

            smoothValue = Mathf.SmoothStep(0, 1, t);
            transform.localPosition = startPosition + new Vector3(_direction.x * smoothValue, _direction.y * smoothValue, 0);
        }

        transform.localPosition = startPosition + new Vector3(_direction.x, _direction.y, 0);
        m_Dashing = false;
    }

    private IEnumerator ReceiveDamageMat()
    {
        m_Material.SetFloat("_InterpolationValue", 1);
        yield return new WaitForSeconds(0.05f);
        m_Material.SetFloat("_InterpolationValue", 0);
    }
}
