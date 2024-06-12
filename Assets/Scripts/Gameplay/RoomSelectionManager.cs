using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static LevelManager;

public class RoomSelectionManager : MonoBehaviour
{
    [SerializeField]
    private RoomSelectionHandler m_SelectorReference;
    [SerializeField]
    private List<Pair<MapNode.ROOM_TYPE, Sprite>> m_SpriteMapper;
    [SerializeField]
    private Transform m_SelectorParent;
    [SerializeField]
    private Transform m_SelectorRoot;
    [SerializeField]
    private Transform m_PanelHider;
    [SerializeField]
    private RectTransform m_PanelContainer;
    [SerializeField]
    private CanvasGroup m_Canvas;


    public IEnumerator SetupSelection(List<MapNode> rooms)
    {
        Image image = m_PanelHider.GetComponent<Image>();
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
        m_Canvas.alpha = 0;

        yield return null;

        m_PanelContainer.gameObject.SetActive(true);
        m_SelectorRoot.gameObject.SetActive(true);
        m_PanelHider.gameObject.SetActive(true);

        Vector3 endPos = m_PanelContainer.position;
        Vector3 startPos = m_PanelContainer.position - new Vector3(0, 7, 0);

        float t = 0;
        while (t < 1)
        {
            m_PanelContainer.position = Vector3.Lerp(startPos, endPos, t);
            image.color = new Color(image.color.r, image.color.g, image.color.b, t * 0.6f);
            m_Canvas.alpha = t;
            t += Time.deltaTime;
            yield return null;
        }

        m_PanelContainer.localScale = Vector3.one;
        m_PanelContainer.position = endPos;
        m_Canvas.alpha = 1;

        List<Pair<MapNode.ROOM_TYPE, MapNode.RARITY>> currentViewd = new List<Pair<MapNode.ROOM_TYPE, MapNode.RARITY>>();

        int amountExtra = rooms.Count - 3;

        for (int i = 0; i < rooms.Count; ++i)
        {
            if (currentViewd.FindIndex(x => (x.Key == rooms[i].m_RoomType && x.Value == rooms[i].m_Rarity)) != -1)
                continue;

            if (amountExtra > 0 && rooms[i].m_RoomType == MapNode.ROOM_TYPE.ENEMY)
            {
                amountExtra--;
                continue;
            }
            
            RoomSelectionHandler handler = Instantiate(m_SelectorReference, m_SelectorParent);
            yield return handler.Initialize(i, m_SpriteMapper.Find((x) => x.Key == rooms[i].m_RoomType).Value, RoomToText(rooms[i]));
            currentViewd.Add(new Pair<MapNode.ROOM_TYPE, MapNode.RARITY>(rooms[i].m_RoomType, rooms[i].m_Rarity));
        }

        yield return null;
    }

    public IEnumerator HideSelection(Action initialize)
    {
        float t = 0;
        Vector3 startPos = m_PanelContainer.position;
        Vector3 endPos = m_PanelContainer.position - new Vector3(0, 7, 0);

        Image image = m_SelectorRoot.GetComponent<Image>();
        float startAlpha = image.color.a;

        while (t < 1)
        {
            m_PanelContainer.position = Vector3.Lerp(startPos, endPos, t);
            t += Time.deltaTime;
            yield return null;
        }

        Image imagePanel = m_PanelHider.GetComponent<Image>();
        imagePanel.color = new Color(image.color.r, image.color.g, image.color.b, 0.6f);

        t = 0;
        while (t < 1f)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, startAlpha + t * (1-startAlpha));
            imagePanel.color = new Color(imagePanel.color.r, imagePanel.color.g, imagePanel.color.b, 0.6f - 0.6f*t);
            t += Time.deltaTime * 2;
            yield return null;
        }

        yield return null;

        foreach(Transform child in m_SelectorParent)
        {
            Destroy(child.gameObject);
        }

        initialize();
        yield return null;

        t = 0;
        while (t < 1)
        {
            m_Canvas.alpha = 1 - t;
            t += Time.deltaTime;
            yield return null;
        }
        m_Canvas.alpha = 0;

        m_PanelContainer.gameObject.SetActive(false);
        m_SelectorRoot.gameObject.SetActive(false);
        m_PanelHider.gameObject.SetActive(false);

        yield return null;
        image.color = new Color(image.color.r, image.color.g, image.color.b, startAlpha);
        m_PanelContainer.position = startPos;
    }

    private string RoomToText(MapNode node)
    {
        switch (node.m_RoomType)
        {
            case MapNode.ROOM_TYPE.SHOP:
                return "SHOP";
            case MapNode.ROOM_TYPE.ENEMY:
                return "FIGHT (" + RarityToLetter(node.m_Rarity) + ")";
            case MapNode.ROOM_TYPE.ITEM:
                return "ITEM";
            case MapNode.ROOM_TYPE.BOSS:
                return "BOSS (" + RarityToLetter(node.m_Rarity) + ")";
            case MapNode.ROOM_TYPE.START:
                return "INIT";
        }

        return string.Empty;
    }

    private string RarityToLetter(MapNode.RARITY rarity)
    {
        switch (rarity)
        {
            case MapNode.RARITY.EASY:
                return "E";
            case MapNode.RARITY.MEDIUM:
                return "N";
            case MapNode.RARITY.HARD:
                return "H";
        }
        return "ERROR";
    }
}
