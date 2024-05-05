using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : BoardElement
{
    [SerializeField]
    private float m_DashSpeed = 1;
    private bool m_Dashing = false;

    #region INHERITED_METHODS
    public override void Initialize()
    {
    }

    public override void SetPosition(int x, int y)
    {
        transform.localPosition = new Vector3(x, y, 0);
    }

    public override ELEMENT_TYPE GetElementType()
    {
        return ELEMENT_TYPE.PLAYER;
    }
    #endregion

    #region PLAYER_ACTIONS
    public void Dash(Vector2Int _direction)
    {
        StartCoroutine(DashCoroutine(_direction));
    }

    private IEnumerator DashCoroutine(Vector2Int _direction)
    {
        if (m_Dashing)
            yield break;

        m_Dashing = true;

        Vector3 startPosition = transform.position;
        float t = 0;
        float smoothValue;

        while (t < 1)
        {
            yield return null;

            t += Time.deltaTime * m_DashSpeed;

            smoothValue = Mathf.SmoothStep(0, 1, t);
            transform.position = startPosition + new Vector3(_direction.x * smoothValue, _direction.y * smoothValue, 0);
        }

        transform.position = startPosition + new Vector3(_direction.x, _direction.y, 0);

        m_Dashing = false;
    }
    #endregion

    #region DEBUG
    private void OnGUI()
    {
        if (GUI.Button(new Rect(0,0,200,200), "Dash left"))
        {
            Dash(new Vector2Int(-1, 0));
        }
    }
    #endregion
}
