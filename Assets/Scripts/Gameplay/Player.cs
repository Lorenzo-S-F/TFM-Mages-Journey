using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Player : BoardElement
{
    [SerializeField]
    private Transform m_Transform;
    [SerializeField]
    private HPHandler m_HPHandler;
    [SerializeField]
    private GoldHandler m_GoldHandler;
    [SerializeField]
    private SpecialAttackHandler m_SpecialAttackHandler;
    [SerializeField]
    private AudioClip m_DashAudio;

    private GameplayManagers m_GameplayManagers;
    private RoomEntity m_CurrentPlayerEntity;
    private EntityStats m_CurrentEntityStats;
    private Coroutine m_DashCoroutine;
    private GameManager m_GameManager;
    private Material m_Material;

    private bool m_Dashing = false;
    private bool m_Initialized = false;
    private float m_DashTimeInmunnity = 0;
    private float m_ShootCooldown = 0f;
    private int m_TotalGold = 0;

    private void Awake()
    {
        m_GameplayManagers = GameplayManagers.Instance;
        m_GameManager = GameplayManagers.Instance.m_GameManager;
    }

    private void Update()
    {
        if (m_ShootCooldown > 0)
            m_ShootCooldown -= Time.deltaTime * m_GameplayManagers.m_TimeMultiplier;
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
        m_CurrentEntityStats.m_AttackProjectileTime = m_CurrentPlayerEntity.m_Entity.m_EntityStats.m_AttackProjectileTime;
        m_CurrentEntityStats.m_AttackSpeed = m_CurrentPlayerEntity.m_Entity.m_EntityStats.m_AttackSpeed;
        m_CurrentEntityStats.m_ShotSpeed = m_CurrentPlayerEntity.m_Entity.m_EntityStats.m_ShotSpeed;
        m_CurrentEntityStats.m_Damage = m_CurrentPlayerEntity.m_Entity.m_EntityStats.m_Damage;
        m_CurrentEntityStats.m_Speed = m_CurrentPlayerEntity.m_Entity.m_EntityStats.m_Speed;
        m_CurrentEntityStats.m_HP = m_CurrentPlayerEntity.m_Entity.m_EntityStats.m_HP;

        entity.m_Entity = Instantiate(entity.m_Entity);

        m_TotalGold = entity.m_Entity.m_GoldOnDeath;
        m_GoldHandler.SetCurrentGold(m_TotalGold);

        m_HPHandler.Initialize(m_CurrentEntityStats.m_HP);
    }

    internal bool IsDashing()
    {
        return m_Dashing;
    }

    internal int GetCurrentGold()
    {
        return m_TotalGold;
    }

    internal void AddGold(int gold)
    {
        m_TotalGold += gold;
        m_GoldHandler.SetCurrentGold(m_TotalGold);
    }

    public override Vector2Int GetPosition()
    {
        return m_CurrentPlayerEntity.m_Position;
    }

    public void InitializeTransform(Transform playerTransform)
    {
        m_Transform = playerTransform;
        m_Material = m_Transform.GetChild(0).GetComponent<SpriteRenderer>().material;
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
        Shoot(m_CurrentPlayerEntity.m_Entity.m_AttackData[0]);
    }

    private void Shoot(EntityAttack attackData)
    {
        if (m_ShootCooldown > 0)
            return;

        if (m_Dashing || m_Transform == null)
            return;

        Vector2Int dir = m_GameManager.GetMostAlignedDirection(this);

        StartCoroutine(ShotSystem.ShootPattern(
            m_CurrentPlayerEntity.m_Entity.m_ShootSound,
            attackData.m_AttackPattern,
            m_CurrentPlayerEntity.m_Entity.m_AttackData[0].m_Projectile,
            this,
            dir,
            1,
            GameplayManagers.Instance.m_GameManager.m_ProjectilesTransform,
            (parent, projectile, distanceToEntity, bulletParent, rotatedDisplacement, direction) =>
            {
                GameObject attack = Instantiate(projectile, m_Transform.position + rotatedDisplacement * distanceToEntity, Quaternion.identity, bulletParent);
                var bullets = attack.GetComponentsInChildren<Attack>().ToList();

                for (int i = 0; i < bullets.Count; ++i)
                    bullets[i].Initialize(attackData, direction, m_CurrentEntityStats, "Player", m_CurrentPlayerEntity.m_Entity.m_AttackModifications);
            }
            ));
    }

    public void Dash(Vector2Int _direction)
    {
        Vector2Int expectedDirection = m_CurrentPlayerEntity.m_Position + _direction;
        if (m_GameManager.IsValidPosition(m_CurrentPlayerEntity.m_Entity, expectedDirection.x, expectedDirection.y))
        {
            MainManagers.Instance.m_AudioManager.PlaySFXSound(m_DashAudio);
            m_DashCoroutine = StartCoroutine(DashCoroutine(_direction));
        }
    }

    internal bool IsDashInmune()
    {
        if (m_DashTimeInmunnity > Time.time)
            return true;

        return false;
    }

    private IEnumerator DashCoroutine(Vector2Int _direction)
    {
        if (m_Dashing)
            yield break;

        m_DashTimeInmunnity = Time.time + ((1.0f / m_CurrentEntityStats.m_Speed) * 0.8f);

        m_CurrentPlayerEntity.m_Position += _direction;
        m_Dashing = true;

        Vector3 startPosition = m_Transform.localPosition;
        float t = 0;
        float smoothValue;

        while (t < 1)
        {
            yield return null;

            t += Time.deltaTime * m_CurrentEntityStats.m_Speed * m_GameplayManagers.m_TimeMultiplier;

            smoothValue = Mathf.SmoothStep(0, 1, t);
            m_Transform.localPosition = startPosition + new Vector3(_direction.x * smoothValue, _direction.y * smoothValue, 0);
        }

        m_Transform.localPosition = startPosition + new Vector3(_direction.x, _direction.y, 0);

        m_DashCoroutine = null;
        m_Dashing = false;
    }

    public override void ApplyDamage(float damage)
    {
        MainManagers.Instance.m_AudioManager.PlaySFXSound(m_CurrentPlayerEntity.m_Entity.m_DamageSound);

        m_CurrentEntityStats.m_HP -= Mathf.RoundToInt(damage);
        m_HPHandler.ModifyCurrentHP(m_CurrentEntityStats.m_HP);
        if (m_CurrentEntityStats.m_HP <= 0)
        {
            Destroy(gameObject);
        }
        else
            StartCoroutine(ReceiveDamageMat());
    }
#endregion

#region EXTERNAL_ACTIONS

    public void ApplyItem(Item item)
    {
        foreach (var type in item.m_ItemType)
        {
            switch (type)
            {
                case Item.ITEM_TYPE.STATS_MODIFIER_INCREASER:
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

                    m_CurrentEntityStats.m_AttackProjectileTime += item.m_PlaneStatModifiers.m_AttackProjectileTime;
                    m_CurrentPlayerEntity.m_Entity.m_EntityStats.m_AttackProjectileTime += item.m_PlaneStatModifiers.m_AttackProjectileTime;
                    break;
                case Item.ITEM_TYPE.STATS_MODIFIER_MULTIPLIER:
                    m_CurrentEntityStats.m_AttackProjectileTime += m_CurrentPlayerEntity.m_Entity.m_EntityStats.m_AttackProjectileTime * item.m_PlaneStatModifiers.m_AttackProjectileTime;
                    m_CurrentEntityStats.m_AttackSpeed += m_CurrentPlayerEntity.m_Entity.m_EntityStats.m_AttackSpeed * item.m_PlaneStatModifiers.m_AttackSpeed;
                    m_CurrentEntityStats.m_ShotSpeed += m_CurrentPlayerEntity.m_Entity.m_EntityStats.m_ShotSpeed * item.m_PlaneStatModifiers.m_ShotSpeed;
                    m_CurrentEntityStats.m_Damage += m_CurrentPlayerEntity.m_Entity.m_EntityStats.m_Damage * item.m_PlaneStatModifiers.m_Damage;
                    m_CurrentEntityStats.m_Speed += m_CurrentPlayerEntity.m_Entity.m_EntityStats.m_Speed * item.m_PlaneStatModifiers.m_Speed;
                    m_CurrentEntityStats.m_HP += m_CurrentPlayerEntity.m_Entity.m_EntityStats.m_HP * item.m_PlaneStatModifiers.m_HP;

                    m_HPHandler.ModifyAmount(m_CurrentPlayerEntity.m_Entity.m_EntityStats.m_HP, m_CurrentEntityStats.m_HP);
                    break;
                case Item.ITEM_TYPE.SHOT_MODIFIER:
                    m_CurrentPlayerEntity.m_Entity.m_AttackModifications.Add(item.m_Modification);
                    break;
                case Item.ITEM_TYPE.SHOT_SUBSTITUTE:
                    item.m_NewAttack.m_Projectile = m_CurrentPlayerEntity.m_Entity.m_AttackData[0].m_Projectile;
                    m_CurrentPlayerEntity.m_Entity.m_AttackData[0] = item.m_NewAttack;
                    break;
                case Item.ITEM_TYPE.HEAL:
                    m_CurrentEntityStats.m_HP = Mathf.Min(m_CurrentEntityStats.m_HP + item.m_HealAmount, m_CurrentPlayerEntity.m_Entity.m_EntityStats.m_HP);
                    m_HPHandler.ModifyCurrentHP(m_CurrentEntityStats.m_HP);
                    break;
                case Item.ITEM_TYPE.HABILITY:
                    m_SpecialAttackHandler.Initialize(item.m_ItemSprite, item.m_NewHability, this);
                    break;
            }
        }
    }

    public void ShootSpecialAttack()
    {
        Shoot(m_SpecialAttackHandler.GetSpecialAttack());
    }

    private IEnumerator ReceiveDamageMat()
    {
        m_Material.SetFloat("_InterpolationValue", 1);
        yield return new WaitForSeconds(0.05f);
        m_Material.SetFloat("_InterpolationValue", 0);
    }

    #endregion

    public override Entity GetEntity()
    {
        return m_CurrentPlayerEntity.m_Entity;
    }
}
