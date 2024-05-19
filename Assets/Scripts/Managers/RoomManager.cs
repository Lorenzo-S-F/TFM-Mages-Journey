using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [SerializeField]
    private Vector2Int m_RoomSize;

    [SerializeField]
    private List<ValidEncounter> m_AvaliablePositions;

    [SerializeField]
    private List<ItemSlotHandler> m_AvaliableSlots;

    [SerializeField]
    private Transform m_PlayerTransform;
    [SerializeField]
    private Transform m_GridTransform;
    [SerializeField]
    private LevelManager.MapNode.ROOM_TYPE m_RoomType;
    private GameplayManagers m_GamepolayManagers;

    private void Awake()
    {
        m_GamepolayManagers = GameplayManagers.Instance;
    }

    public void Initialize()
    {
        switch (m_RoomType)
        {
            case LevelManager.MapNode.ROOM_TYPE.SHOP:
                break;
            case LevelManager.MapNode.ROOM_TYPE.ITEM:

                var avaliableItems = m_GamepolayManagers.m_LevelManager.GetItemPool();
                foreach(var slot in m_AvaliableSlots)
                {
                    int itemNumber = RandomNumberGenerator.GetInt32(0, avaliableItems.Count);
                    slot.SetItem(avaliableItems[itemNumber].Value);
                    avaliableItems.RemoveAt(itemNumber);
                }

                break;
        }
    }

    public Vector2Int GetRoomSize()
    {
        return m_RoomSize;
    }

    public Transform GetPlayerTransform()
    {
        return m_PlayerTransform;
    }

    public Transform GetGridTransform()
    {
        return m_GridTransform;
    }

    public ValidEncounter GetEncounter(LevelManager.MapNode.RARITY rarity)
    {
        List<ValidEncounter> avaliableEncounters = new List<ValidEncounter>();
        
        while (rarity != LevelManager.MapNode.RARITY.UNDEFINED && avaliableEncounters.Count == 0)
        {
            avaliableEncounters = m_AvaliablePositions.FindAll(x => x.m_EncounterRarity == rarity);
            rarity -= 1;
        }

        return avaliableEncounters[System.Security.Cryptography.RandomNumberGenerator.GetInt32(0, avaliableEncounters.Count)];
    }

    public LevelManager.MapNode.ROOM_TYPE GetRoomType()
    {
        return m_RoomType;
    }

    [Serializable]
    public class ValidEncounter
    {
        public Vector2Int m_PlayerInitialState;
        public LevelManager.MapNode.RARITY m_EncounterRarity;
        public List<MapEnemy> m_RoomEnemies;

        [Serializable]
        public class MapEnemy
        {
            public RoomEntity m_EnemyData;
            public Vector2Int m_StartPosition;
        }
    }
}
