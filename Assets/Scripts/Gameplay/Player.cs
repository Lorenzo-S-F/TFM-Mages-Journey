using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : BoardElement
{
    [SerializeField]
    private float m_DashSpeed = 1;
    [SerializeField]
    private Transform m_Transform;

    private bool m_Dashing = false;
    private Coroutine m_DashCoroutine;
    private RoomEntity m_CurrentPlayerEntity;
    private GameManager m_GameManager;

    private void Awake()
    {
        m_GameManager = GameplayManagers.Instance.m_GameManager;
    }

    public RoomEntity GetRoomEntity()
    {
        return m_CurrentPlayerEntity;
    }

    #region INHERITED_METHODS
    public override void Initialize(RoomEntity entity)
    {
        m_CurrentPlayerEntity = entity;
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
        attack.GetComponent<Attack>().Initialize(m_CurrentPlayerEntity.m_Entity.m_AttackData[0], dir, m_CurrentPlayerEntity.m_Entity.m_EntityStats, "Player");
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

            t += Time.deltaTime * m_DashSpeed;

            smoothValue = Mathf.SmoothStep(0, 1, t);
            m_Transform.localPosition = startPosition + new Vector3(_direction.x * smoothValue, _direction.y * smoothValue, 0);
        }

        m_Transform.localPosition = startPosition + new Vector3(_direction.x, _direction.y, 0);

        m_DashCoroutine = null;
        m_Dashing = false;
    }

    public override void ApplyDamage(float damage)
    {
        m_CurrentPlayerEntity.m_Entity.m_EntityStats.m_HP -= damage;
        if (m_CurrentPlayerEntity.m_Entity.m_EntityStats.m_HP <= 0)
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region DEBUG
    private void OnGUI()
    {
        if (GUI.Button(new Rect(0,0,200,100), "Dash left"))
        {
            Dash(new Vector2Int(-1, 0));
        }

        if (GUI.Button(new Rect(0, 200, 200, 100), "Dash right"))
        {
            Dash(new Vector2Int(1, 0));
        }

        if (GUI.Button(new Rect(0, 400, 200, 100), "Dash top"))
        {
            Dash(new Vector2Int(0, 1));
        }

        if (GUI.Button(new Rect(0, 600, 200, 100), "Dash bottom"))
        {
            Dash(new Vector2Int(0, -1));
        }
    }
    #endregion
}
