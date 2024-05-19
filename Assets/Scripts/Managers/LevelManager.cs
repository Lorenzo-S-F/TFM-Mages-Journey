using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    public List<Pair<int, RoomEntity>> m_AvaliablePlayers;
    public List<Pair<int, RoomManager>> m_EnemyRooms;
    public List<Pair<int, RoomManager>> m_ShopRooms;
    public List<Pair<int, RoomManager>> m_ItemRooms;
    public List<Pair<int, ItemSlotHandler.Item>> m_AvaliableItems;
    public List<Pair<int, RoomManager>> m_BossRoom;
    public GenerationSpecs m_GenerationSpecs;

    internal RoomEntity GetPlayerBase(int index)
    {
        return m_AvaliablePlayers[index].Value;
    }

    public int m_MapSizeX;
    public int m_MapSizeY;

    public MapNode[,] m_Map;
    private int m_CurrentGameLayer = 0;
    private int m_CurrentGameWorld = 0;

    public void GenerateLevel()
    {
        m_Map = new MapNode[m_MapSizeX, m_MapSizeY];

        MapNode start = new MapNode();
        MapNode end = new MapNode();

        end.m_RoomType = MapNode.ROOM_TYPE.BOSS;
        end.m_Rarity = MapNode.RARITY.HARD;
        start.m_RoomType = MapNode.ROOM_TYPE.START;

        m_Map[m_MapSizeX / 2, 0] = start;
        m_Map[m_MapSizeX / 2, m_MapSizeY - 1] = end;

        // Fixed Generation
        // Two item rooms
        int generated = 0;
        while (generated < 2)
        {
            int x = RandomNumberGenerator.GetInt32(0, m_MapSizeX);
            int y = RandomNumberGenerator.GetInt32(1, m_MapSizeY - 1);
            if (m_Map[x,y] == null)
            {
                MapNode newNode = new MapNode();
                newNode.m_Rarity = m_GenerationSpecs.GetRandomRarity();
                newNode.m_RoomType = MapNode.ROOM_TYPE.ITEM;
                m_Map[x, y] = newNode;
                generated++;
            }
        }

        // Two shop rooms
        generated = 0;
        List<int> usedY = new List<int>();
        while (generated < 2)
        {
            int x = RandomNumberGenerator.GetInt32(0, m_MapSizeX);
            int y = RandomNumberGenerator.GetInt32(1, m_MapSizeY - 1);

            if (m_Map[x, y] == null && (usedY.FindIndex(element => element == y) == -1))
            {
                MapNode newNode = new MapNode();
                newNode.m_Rarity = m_GenerationSpecs.GetRandomRarity();
                newNode.m_RoomType = MapNode.ROOM_TYPE.ENEMY;
                m_Map[x, y] = newNode;
                usedY.Add(y);
                generated++;
            }
        }

        // Random Generation
        for (int i = 0; i < m_MapSizeX; ++i)
        {
            MapNode prev = start;
            for (int j = 1; j < m_MapSizeY - 1; ++j)
            {
                if (!m_GenerationSpecs.RoomGenerated())
                    continue;

                if (m_Map[i,j] == null)
                {
                    MapNode newNode = new MapNode();
                    newNode.m_PrevNodes.Add(prev);
                    newNode.m_Rarity = m_GenerationSpecs.GetRandomRarity();
                    newNode.m_RoomType = m_GenerationSpecs.GetRandomRoom();
                    m_Map[i, j] = newNode;
                }
                else
                    m_Map[i, j].m_PrevNodes.Add(prev);

                prev = m_Map[i, j];
            }
            end.m_PrevNodes.Add(prev);
        }

#if UNITY_EDITOR
        string debugReume = "\n";
        for (int j = 0; j < m_MapSizeY; ++j)
        {
            for (int i = 0; i < m_MapSizeX; ++i)
            {
                if (m_Map[i,j] == null)
                    debugReume += "9";
                else
                    debugReume += (int)m_Map[i,j].m_RoomType;
            }
            debugReume += '\n';
        }
        Debug.Log(debugReume);
#endif
    }

    public List<Pair<int, ItemSlotHandler.Item>> GetItemPool()
    {
        return m_AvaliableItems.FindAll(x => x.Key == m_CurrentGameWorld);
    }

    public void RemoveItemFormPool(ItemSlotHandler.Item item)
    {
        m_AvaliableItems.RemoveAll(x => x.Value == item);
    }

    public (RoomManager, MapNode) GetRoom(int room)
    {
        m_CurrentGameLayer++;
        for (int i = 0; i < m_Map.GetLength(0); ++i)
        {
            if (m_Map[i, m_CurrentGameLayer] == null)
                continue;

            if (m_Map[i, m_CurrentGameLayer] != null && room == 0)
            {
                List<Pair<int, RoomManager>> rooms = new List<Pair<int, RoomManager>>();
                switch (m_Map[i, m_CurrentGameLayer].m_RoomType)
                {
                    case MapNode.ROOM_TYPE.SHOP:
                        rooms = m_ShopRooms.FindAll(x => x.Key == m_CurrentGameWorld);
                        break;
                    case MapNode.ROOM_TYPE.ENEMY:
                        rooms = m_EnemyRooms.FindAll(x => x.Key == m_CurrentGameWorld);
                        break;
                    case MapNode.ROOM_TYPE.ITEM:
                        rooms = m_ItemRooms.FindAll(x => x.Key == m_CurrentGameWorld);
                        break;
                    case MapNode.ROOM_TYPE.BOSS:
                        rooms = m_BossRoom.FindAll(x => x.Key == m_CurrentGameWorld);
                        break;
                    case MapNode.ROOM_TYPE.START:
                        break;
                }

                return (rooms[RandomNumberGenerator.GetInt32(0, rooms.Count)].Value, m_Map[i, m_CurrentGameLayer]);
            }

            room--;
        }

        return (null, null);
    }

    public List<MapNode> GetNextLayerMapNodes()
    {
        List<MapNode> nextMapNodes = new List<MapNode>();

        if (m_Map.GetLength(1) == m_CurrentGameLayer + 1)
        {
            MainManagers.Instance.m_LoadingHandler.LoadScene(LoadingHandler.SCENE.MENUS);
            return nextMapNodes;
        }

        while (nextMapNodes.Count == 0)
        {
            for (int i = 0; i < m_MapSizeX; ++i)
            {
                if (m_Map[i, m_CurrentGameLayer + 1] != null)
                    nextMapNodes.Add(m_Map[i, m_CurrentGameLayer + 1]);
            }

            if (nextMapNodes.Count == 0)
                m_CurrentGameLayer++;
        }

        return nextMapNodes;
    }

    [Serializable]
    public class GenerationSpecs
    {
        [Range(0,1)]
        public float m_RoomProbability;
        public List<Pair<MapNode.ROOM_TYPE, float>> m_Probabilities;
        public List<Pair<MapNode.RARITY, float>> m_RarityProbabilities;

        public MapNode.ROOM_TYPE GetRandomRoom()
        {
            return MapNode.ROOM_TYPE.ENEMY;

            float total = 0;
            for (int i = 0; i < m_Probabilities.Count; ++i)
            {
                total += m_Probabilities[i].Value;
            }

            float random = RandomNumberGenerator.GetInt32(int.MaxValue) / (float)int.MaxValue;
            random *= total;

            for (int i = 0; i < m_Probabilities.Count; ++i)
            {
                if (random > m_Probabilities[i].Value)
                    random -= m_Probabilities[i].Value;
                else
                    return m_Probabilities[i].Key;
            }

            return MapNode.ROOM_TYPE.UNDEFINED;
        }

        public bool RoomGenerated()
        {
            if (RandomNumberGenerator.GetInt32(int.MaxValue) / (float)int.MaxValue < m_RoomProbability)
                return true;

            return false;
        }

        internal MapNode.RARITY GetRandomRarity()
        {
            float total = 0;
            for (int i = 0; i < m_RarityProbabilities.Count; ++i)
            {
                total += m_RarityProbabilities[i].Value;
            }

            float random = RandomNumberGenerator.GetInt32(int.MaxValue) / (float)int.MaxValue;
            random *= total;

            for (int i = 0; i < m_RarityProbabilities.Count; ++i)
            {
                if (random > m_RarityProbabilities[i].Value)
                    random -= m_RarityProbabilities[i].Value;
                else
                    return m_RarityProbabilities[i].Key;
            }

            return MapNode.RARITY.UNDEFINED;
        }
    }

    [Serializable]
    public class Pair<T1, T2>
    {
        public T1 Key;
        public T2 Value;

        public Pair() { }

        public Pair(T1 key, T2 value)
        {
            Key = key;
            Value = value;
        }
    }

    public class MapNode
    {
        public List<MapNode> m_PrevNodes = new List<MapNode>();
        public ROOM_TYPE m_RoomType = ROOM_TYPE.UNDEFINED;
        public RARITY m_Rarity = RARITY.UNDEFINED;

        public enum RARITY
        {
            UNDEFINED,
            EASY,
            MEDIUM,
            HARD
        }

        public enum ROOM_TYPE
        {
            UNDEFINED,
            SHOP,
            ENEMY,
            ITEM,
            BOSS,
            START
        }        
    }

    #region DEBUG
#if UNITY_EDITOR
    private void OnGUI()
    {

        /*if (GUI.Button(new Rect(0, 800, 200, 100), "Generate Level"))
        {
            GenerateLevel();
        }*/
    }
#endif
    #endregion

}
