using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : BoardElement
{
    [SerializeField]
    private Transform m_Transform;
    [SerializeField]
    private HPHandler m_HPHandler;

    private bool m_Dashing = false;
    private Coroutine m_DashCoroutine;
    private RoomEntity m_CurrentPlayerEntity;
    private EntityStats m_CurrentEntityStats;
    private GameManager m_GameManager;
    private bool m_Initialized = false;

    private void Awake()
    {
        m_GameManager = GameplayManagers.Instance.m_GameManager;
    }

    public EntityStats GetPlayerStats()
    {
        return m_CurrentEntityStats;
    }

    public RoomEntity GetRoomEntity()
    {
        return m_CurrentPlayerEntity;
    }

    #region INHERITED_METHODS
    public override void Initialize(RoomEntity entity)
    {
        if (m_Initialized)
            return;

        m_Initialized = true;

        m_CurrentPlayerEntity = entity;

        m_CurrentEntityStats = new EntityStats();
        m_CurrentEntityStats.m_AttackSpeed = m_CurrentPlayerEntity.m_Entity.m_EntityStats.m_AttackSpeed;
        m_CurrentEntityStats.m_Damage = m_CurrentPlayerEntity.m_Entity.m_EntityStats.m_Damage;
        m_CurrentEntityStats.m_HP = m_CurrentPlayerEntity.m_Entity.m_EntityStats.m_HP;
        m_CurrentEntityStats.m_ShotSpeed = m_CurrentPlayerEntity.m_Entity.m_EntityStats.m_ShotSpeed;
        m_CurrentEntityStats.m_Speed = m_CurrentPlayerEntity.m_Entity.m_EntityStats.m_Speed;

        m_HPHandler.Initialize(m_CurrentEntityStats.m_HP);
    }

    public override Vector2Int GetPosition()
    {
        return m_CurrentPlayerEntity.m_Position;
    }

    public void InitializeTransform(Transform playerTransform)
    {
        m_Transform = playerTransform;
    }        

    public override void SetPosition(int x, int y)
    {
        if (m_DashCoroutine != null)
        {
            StopCoroutine(m_DashCoroutine);
            m_DashCoroutine = null;
            m_Dashing = false;
        }
        m_Transform.localPosition = new Vector3(x, y, 0);
    }

    public override ELEMENT_TYPE GetElementType()
    {
        return ELEMENT_TYPE.PLAYER;
    }
    #endregion

    #region PLAYER_ACTIONS

    public void Shoot()
    {
        if (m_Dashing || m_Transform == null)
            return;

        Vector2Int dir = m_GameManager.GetMostAlignedDirection(this);
        GameObject attack = Instantiate(m_CurrentPlayerEntity.m_Entity.m_AttackData[0].m_AttackType.m_Projectile, GameplayManagers.Instance.m_GameManager.m_ProjectilesTransform);
        attack.transform.localPosition = m_Transform.position;

        var bullets = attack.GetComponentsInChildren<Attack>().ToList();
        var extraAttack = attack.GetComponent<Attack>();

        if (extraAttack != null)
            bullets.Add(extraAttack);

        for (int i = 0; i < bullets.Count; ++i)
            bullets[i].Initialize(m_CurrentPlayerEntity.m_Entity.m_AttackData[0], dir, m_CurrentEntityStats, "Player");

        if (dir.x != 0)
            attack.gameObject.transform.Rotate(new Vector3(0, 0, 90));
    }

    public void Dash(Vector2Int _direction)
    {
        Vector2Int expectedDirection = m_CurrentPlayerEntity.m_Position + _direction;
        if (m_GameManager.IsValidPosition(expectedDirection.x, expectedDirection.y))
        {
            m_DashCoroutine = StartCoroutine(DashCoroutine(_direction));
        }
    }

    private IEnumerator DashCoroutine(Vector2Int _direction)
    {
        if (m_Dashing)
            yield break;

        m_CurrentPlayerEntity.m_Position += _direction;
        m_Dashing = true;

        Vector3 startPosition = m_Transform.localPosition;
        float t = 0;
        float smoothValue;

        while (t < 1)
        {
            yield return null;

            t += Time.deltaTime * m_CurrentEntityStats.m_Speed;

            smoothValue = Mathf.SmoothStep(0, 1, t);
            m_Transform.localPosition = startPosition + new Vector3(_direction.x * smoothValue, _direction.y * smoothValue, 0);
        }

        m_Transform.localPosition = startPosition + new Vector3(_direction.x, _direction.y, 0);

        m_DashCoroutine = null;
        m_Dashing = false;
    }

    public override void ApplyDamage(int damage)
    {
        m_CurrentEntityStats.m_HP -= damage;
        m_HPHandler.ModifyCurrentHP(m_CurrentEntityStats.m_HP);
        if (m_CurrentEntityStats.m_HP <= 0)
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region EXTERNAL_ACTIONS

    public void ApplyItem(ItemSlotHandler.Item item)
    {
        m_CurrentEntityStats.m_AttackSpeed += item.m_PlaneStatModifiers.m_AttackSpeed;
        m_CurrentPlayerEntity.m_Entity.m_EntityStats.m_AttackSpeed += item.m_PlaneStatModifiers.m_AttackSpeed;

        m_CurrentEntityStats.m_Damage += item.m_PlaneStatModifiers.m_Damage;
        m_CurrentPlayerEntity.m_Entity.m_EntityStats.m_Damage += item.m_PlaneStatModifiers.m_Damage;

        m_CurrentEntityStats.m_HP += item.m_PlaneStatModifiers.m_HP;
        m_CurrentPlayerEntity.m_Entity.m_EntityStats.m_HP += item.m_PlaneStatModifiers.m_HP;
        m_HPHandler.ModifyAmount(m_CurrentPlayerEntity.m_Entity.m_EntityStats.m_HP, m_CurrentEntityStats.m_HP);

        m_CurrentEntityStats.m_ShotSpeed += item.m_PlaneStatModifiers.m_ShotSpeed;
        m_CurrentPlayerEntity.m_Entity.m_EntityStats.m_ShotSpeed += item.m_PlaneStatModifiers.m_ShotSpeed;

        m_CurrentEntityStats.m_Speed += item.m_PlaneStatModifiers.m_Speed;
        m_CurrentPlayerEntity.m_Entity.m_EntityStats.m_Speed += item.m_PlaneStatModifiers.m_Speed;
    }

    #endregion
}
