using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using static Entity;

public class Attack : MonoBehaviour
{
    private EntityAttack m_Attack;
    private ProjectileStats m_Stats;
    private List<ProjectileModifications> m_Modifications = new List<ProjectileModifications>();

    private Vector3 m_Direction;
    private float m_MaxTimeInScreen = 10f;
    private float m_CurrentTimeInScreen = 0f;
    private string m_SourceTag;

    private bool m_Triggered;
    private HashSet<int> m_HitElements = new HashSet<int>();
    private float m_StartTime;
    private GameplayManagers m_GameplayManager;

    [SerializeField] private SpriteRenderer m_ProjectileImage;
    [SerializeField] private List<ParticleSystem> m_ProjectileParticles;
    [SerializeField] private List<ParticleSystem> m_EndParticles;

    public class ProjectileModifications
    {
        public Modification m_ModificationReference;
        public ProjectileStatsModifications m_StatsModification = new ProjectileStatsModifications();
    }

    public class ProjectileStatsModifications
    {
        public float m_ShotSpeedMultiplier = 1;
        public float m_AngleModification = 1;
        public float m_DamageMultiplier = 1;
        public float m_SizeMultiplier = 1;
    }

    public class ProjectileStats
    {
        public Vector2 m_OriginalDirection;
        public float m_ShotSpeed = 1;
        public float m_Damage = 1;
        public Vector3 m_Size = Vector3.one;
    }

    private int m_Hits;

    public void Initialize(EntityAttack attack, Vector2 direction, EntityStats source, string sourceTag, List<Modification> modifications)
    {
        m_GameplayManager = GameplayManagers.Instance;
        m_StartTime = Time.time + ((float)RandomNumberGenerator.GetInt32(0, 100)) / 100f;
        m_Direction = new Vector3(direction.x, direction.y, 0);
        m_Direction = m_Direction.normalized;
        m_Stats = new ProjectileStats()
        {
            m_Damage = source.m_Damage,
            m_OriginalDirection = direction,
            m_ShotSpeed = source.m_ShotSpeed,
            m_Size = transform.localScale
        };
        foreach (var modification in modifications)
            m_Modifications.Add(new ProjectileModifications() { m_ModificationReference = modification, m_StatsModification = new ProjectileStatsModifications() });
        m_SourceTag = sourceTag;
        m_Attack = attack;
        m_MaxTimeInScreen = source.m_AttackProjectileTime;
    }

    private void Update()
    {
        foreach (var modification in m_Modifications)
        {
            if (modification.m_ModificationReference.m_CheckTime != Modification.CHECK_TIME.UPDATE)
                continue;

            ApplyModification(modification);
        }

        Vector3 size = m_Stats.m_Size;
        foreach (var modification in m_Modifications)
            size *= modification.m_StatsModification.m_SizeMultiplier;
        transform.localScale = size;

        float shotSpeed = m_Stats.m_ShotSpeed;
        foreach (var modification in m_Modifications)
            shotSpeed *= modification.m_StatsModification.m_ShotSpeedMultiplier;

        if (m_Triggered)
            return;

        if (m_MaxTimeInScreen > m_CurrentTimeInScreen)
        {
            transform.position += (m_Direction * shotSpeed * Time.deltaTime * m_GameplayManager.m_TimeMultiplier);
            m_CurrentTimeInScreen += (Time.deltaTime * m_GameplayManager.m_TimeMultiplier);
        }
        else
        {
            m_Triggered = true;
            StartCoroutine(OnEnd());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (m_Triggered)
            return;

        if (collision.tag != m_SourceTag)
        {
            if (collision.tag == "Enemy")
                ApplyHitEffect(collision.gameObject, (value) => { collision.gameObject.GetComponent<BoardElement>().ApplyDamage(value); });      
            else if (collision.tag == "Player")
            {
                if (!GameplayManagers.Instance.m_GameManager.m_PlayerBase.IsDashInmune() && !GameplayManagers.Instance.m_GameManager.m_PlayerBase.IsPerfectDashing())
                {
                    ApplyHitEffect(collision.gameObject, (value) => { GameplayManagers.Instance.m_GameManager.m_PlayerBase.ApplyDamage(value); });
                }
                else
                {
                    m_HitElements.Add(collision.gameObject.GetInstanceID());
                    if (!GameplayManagers.Instance.m_GameManager.m_PlayerBase.IsPerfectDashing())
                        m_GameplayManager.m_GameManager.SlowTime();
                }
            }
            else if (
                   collision.tag == "Obstacle" 
                && m_Attack.m_ThrowType == EntityAttack.THROW_TYPE.GROUND 
                && m_Modifications.FindIndex(x => x.m_ModificationReference.m_Modification == Modification.MODIFICATION.OBSTACLE_INMUNE) == -1)
            {
                m_Triggered = true;
                StartCoroutine(OnEnd());
            }
        }
    }

    private IEnumerator OnEnd()
    {
        m_Direction = Vector3.zero;

        foreach (var particle in m_ProjectileParticles)
            particle.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        foreach (var particle in m_EndParticles)
            particle.Play();

        m_ProjectileImage.enabled = false;

        yield return new WaitForSeconds(1);

        Destroy(gameObject);
    }

    private void PlayHitParticles()
    {
        foreach (var particle in m_EndParticles)
            particle.Play();
    }

    private void ApplyHitEffect(GameObject gameObject, Action<float> onHit)
    {
        if (m_HitElements.Contains(gameObject.GetInstanceID()))
            return;

        m_HitElements.Add(gameObject.GetInstanceID());

        bool destroyBlocked = false;
        foreach(var modification in m_Modifications)
        {
            if (modification.m_ModificationReference.m_CheckTime != Modification.CHECK_TIME.HIT)
                continue;

            destroyBlocked = destroyBlocked || ApplyModification(modification);
        }

        float finalDamage = m_Stats.m_Damage;
        foreach (var modification in m_Modifications)
            finalDamage *= modification.m_StatsModification.m_DamageMultiplier;

        onHit(finalDamage);
        m_Hits++;

        if (!destroyBlocked)
        {
            m_Triggered = true;
            StartCoroutine(OnEnd());
        }
        else
            PlayHitParticles();
    }

    private bool ApplyModification(ProjectileModifications modification)
    {
        switch (modification.m_ModificationReference.m_Modification)
        {
            case Modification.MODIFICATION.UNDEFINED:
                return false;
            case Modification.MODIFICATION.DECREASE_ON_HIT:
                switch (m_Hits)
                {
                    case 0:
                        modification.m_StatsModification.m_DamageMultiplier = 0.75f;
                        modification.m_StatsModification.m_SizeMultiplier = 0.75f;
                        break;
                    case 1:
                        modification.m_StatsModification.m_SizeMultiplier = 0.5f;
                        break;
                    case 2:
                        modification.m_StatsModification.m_DamageMultiplier = 0.25f;
                        modification.m_StatsModification.m_SizeMultiplier = 0.25f;
                        break;
                    default:
                        modification.m_StatsModification.m_DamageMultiplier = 0f;
                        modification.m_StatsModification.m_SizeMultiplier = 0f;
                        return false;
                }
                return true;
            case Modification.MODIFICATION.SIZE_BEAT:
                modification.m_StatsModification.m_SizeMultiplier = 1 + 0.25f * (float)Math.Sin((Time.time - m_StartTime)* 4);
                break;
            case Modification.MODIFICATION.ENEMY_INMUNE:
                return true;
        }

        return false;
    }
}
