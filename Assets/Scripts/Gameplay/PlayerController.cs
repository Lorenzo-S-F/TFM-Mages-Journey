using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    private Player m_PlayerReference;
    private Vector2 m_StartDragPos;
    private float m_ClickDownTime;
    private float m_ClickCooldown;
    private bool m_BufferedShot;
    private Vector2Int m_BufferedDash = Vector2Int.zero;
    private GameplayManagers m_GameplayManagers;

    private void Awake()
    {
        m_GameplayManagers = GameplayManagers.Instance;
    }

    public void SetPlayer(Player player)
    {
        m_PlayerReference = player;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        m_StartDragPos = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Vector2Int dashDirection = new Vector2Int(0, 0);

        Vector2 endDragPos = eventData.position;
        Vector2 dir = endDragPos - m_StartDragPos;

        float angle = Vector2.Angle(Vector2.up, dir);

        if (dir.x >= 0)
        {
            if (0 <= angle && angle < 45)
            {
                dashDirection = (new Vector2Int(0, 1));
            }
            else if (45 <= angle && angle < 135)
            {
                dashDirection = (new Vector2Int(1, 0));
            }
            else if (135 <= angle && angle <= 180)
            {
                dashDirection = (new Vector2Int(0, -1));
            }
            else
                Debug.LogError("Error on angle. X is positive and angle value is " + angle);
        }
        else
        {
            if (0 <= angle && angle < 45)
            {
                dashDirection = (new Vector2Int(0, 1));
            }
            else if (45 <= angle && angle < 135)
            {
                dashDirection = (new Vector2Int(-1, 0));
            }
            else if (135 <= angle && angle <= 180)
            {
                dashDirection = (new Vector2Int(0, -1));
            }
            else
                Debug.LogError("Error on angle. X is positive and angle value is " + angle);
        }

        if (dashDirection.x != 0 || dashDirection.y != 0)
        {
            if (!m_PlayerReference.IsDashing())
            {
                m_PlayerReference.Dash(dashDirection);
            }
            else
                m_BufferedDash = dashDirection;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        m_ClickDownTime = Time.time;
    }

    public void OnPointerUp(PointerEventData eventData)
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Time.time - m_ClickCooldown > 0.08f * (m_GameplayManagers.m_TimeMultiplier / m_GameplayManagers.m_TimeMultiplier))
        {
            if (Time.time < m_ClickCooldown)
            {
                m_ClickCooldown = Time.time + 0.2f * (m_GameplayManagers.m_TimeMultiplier/ m_GameplayManagers.m_TimeMultiplier);
                m_PlayerReference.Shoot();
                m_BufferedShot = false;
            }
            else
            {
                m_BufferedShot = true;
            }
        }
    }

    private void Update()
    {
        if (m_PlayerReference == null)
            return;

        if (m_BufferedShot && Time.time - m_ClickDownTime > 0.04f * (m_GameplayManagers.m_TimeMultiplier / m_GameplayManagers.m_TimeMultiplier)) 
        { 
            m_ClickDownTime = Time.time + 0.2f;
            m_PlayerReference.Shoot();
            m_BufferedShot = false;
        }

        if (m_BufferedDash.x != 0 || m_BufferedDash.y != 0)
        {
            if (!m_PlayerReference.IsDashing())
            {
                m_PlayerReference.Dash(m_BufferedDash);
                m_BufferedDash = Vector2Int.zero;
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {

    }
}
