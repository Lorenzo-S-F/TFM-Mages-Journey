using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField]
    private Player m_PlayerReference;
    private Vector2 m_StartDragPos;
    //private bool m_Dragging = false;

    public void OnBeginDrag(PointerEventData eventData)
    {
        m_StartDragPos = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Vector2 endDragPos = eventData.position;
        Debug.Log(Vector2.Distance(endDragPos, m_StartDragPos));

        if (true)
        {
            Vector2 dir = endDragPos - m_StartDragPos;

            float angle = Vector2.Angle(Vector2.up, dir);

            if (dir.x >= 0)
            {
                if (0 <= angle && angle < 45)
                {
                    m_PlayerReference.Dash(new Vector2Int(0, 1));
                }
                else if (45 <= angle && angle < 135)
                {
                    m_PlayerReference.Dash(new Vector2Int(1, 0));
                }
                else if (135 <= angle && angle <= 180)
                {
                    m_PlayerReference.Dash(new Vector2Int(0, -1));
                }
                else
                    Debug.LogError("Error on angle. X is positive and angle value is " + angle);
            }
            else
            {
                if (0 <= angle && angle < 45)
                {
                    m_PlayerReference.Dash(new Vector2Int(0, 1));
                }
                else if (45 <= angle && angle < 135)
                {
                    m_PlayerReference.Dash(new Vector2Int(-1, 0));

                }
                else if (135 <= angle && angle <= 180)
                {
                    m_PlayerReference.Dash(new Vector2Int(0, -1));
                }
                else
                    Debug.LogError("Error on angle. X is positive and angle value is " + angle);
            }

        }

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
