using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CommonButtonHandler : Selectable, IPointerClickHandler, IEventSystemHandler
{
    [SerializeField]
    private float m_AnimationTime;
    [SerializeField]
    private List<Sprite> m_OnClickSprites;
    [SerializeField]
    private RectTransform m_ChildObjects;
    [SerializeField]
    private Image m_ButtonImage;
    [SerializeField]
    public UnityEvent m_OnClick;
    [SerializeField]
    private AnimationCurve m_ObjectsCurve;

    private Coroutine m_DownCoroutine;
    private Coroutine m_UpCoroutine;

    protected override void Start()
    {
        base.Start();
        m_ChildObjects.localPosition = new Vector3(m_ChildObjects.localPosition.x, m_ObjectsCurve.Evaluate(0), m_ChildObjects.localPosition.z);
    }

    public void OnPointerClick(PointerEventData _eventData)
    {
        m_OnClick?.Invoke();
    }

    public override void OnPointerDown(PointerEventData _eventData)
    {
        base.OnPointerDown(_eventData);
        if (m_DownCoroutine == null)
            m_DownCoroutine = StartCoroutine(OnPointerDownAnimation());

        if (m_UpCoroutine != null)
        {
            StopCoroutine(m_UpCoroutine);
            m_UpCoroutine = null;
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);

        if (m_UpCoroutine == null)
            m_UpCoroutine = StartCoroutine(OnPointerExitAnimation());

        if (m_DownCoroutine != null)
        {
            StopCoroutine(m_DownCoroutine);
            m_DownCoroutine = null;
        }
    }

    private IEnumerator OnPointerDownAnimation()
    {
        for (int i = 0; i < m_OnClickSprites.Count; ++i)
        {
            m_ChildObjects.localPosition = new Vector3(m_ChildObjects.localPosition.x, m_ObjectsCurve.Evaluate(((float)i / (m_OnClickSprites.Count - 1))), m_ChildObjects.localPosition.z);
            m_ButtonImage.sprite = m_OnClickSprites[i];
            yield return new WaitForSeconds(m_AnimationTime / m_OnClickSprites.Count);
        }

        m_DownCoroutine = null;
    }

    private IEnumerator OnPointerExitAnimation()
    {
        for (int i = m_OnClickSprites.Count - 1; i >= 0; --i)
        {
            m_ChildObjects.localPosition = new Vector3(m_ChildObjects.localPosition.x, m_ObjectsCurve.Evaluate((float)i / (m_OnClickSprites.Count - 1)), m_ChildObjects.localPosition.z);
            m_ButtonImage.sprite = m_OnClickSprites[i];
            yield return new WaitForSeconds(m_AnimationTime / m_OnClickSprites.Count);
        }

        m_UpCoroutine = null;
    }

}
