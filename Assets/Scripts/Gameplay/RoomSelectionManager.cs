using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    private CanvasGroup m_Canvas;

    public IEnumerator SetupSelection(List<MapNode> rooms)
    {
        m_SelectorRoot.gameObject.SetActive(true);
        float t = 0;
        while (t < 1)
        {
            m_Canvas.alpha = t;
            t += Time.deltaTime;
            yield return null;
        }
        m_Canvas.alpha = 1;


        List<Pair<MapNode.ROOM_TYPE, MapNode.RARITY>> currentViewd = new List<Pair<MapNode.ROOM_TYPE, MapNode.RARITY>>();

        for(int i = 0; i < rooms.Count; ++i)
        {
            if (currentViewd.FindIndex(x => (x.Key == rooms[i].m_RoomType && x.Value == rooms[i].m_Rarity)) != -1)
                continue;
            
            RoomSelectionHandler handler = Instantiate(m_SelectorReference, m_SelectorParent);
            yield return handler.Initialize(i, m_SpriteMapper.Find((x) => x.Key == rooms[i].m_RoomType).Value, RoomToText(rooms[i]));
            currentViewd.Add(new Pair<MapNode.ROOM_TYPE, MapNode.RARITY>(rooms[i].m_RoomType, rooms[i].m_Rarity));
        }

        yield return null;
    }

    public IEnumerator HideSelection()
    {
        float t = 0;
        while (t < 1)
        {
            m_Canvas.alpha = 1 - t;
            t += Time.deltaTime;
            yield return null;
        }
        m_Canvas.alpha = 0;

        foreach(Transform child in m_SelectorParent)
        {
            Destroy(child.gameObject);
        }
        m_SelectorRoot.gameObject.SetActive(false);
    }

    private string RoomToText(MapNode node)
    {
        switch (node.m_RoomType)
        {
            case MapNode.ROOM_TYPE.SHOP:
                return "SHOP (" + RarityToLetter(node.m_Rarity) + ")";
            case MapNode.ROOM_TYPE.ENEMY:
                return "FIGHT (" + RarityToLetter(node.m_Rarity) + ")";
            case MapNode.ROOM_TYPE.ITEM:
                return "ITEM (" + RarityToLetter(node.m_Rarity) + ")";
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
