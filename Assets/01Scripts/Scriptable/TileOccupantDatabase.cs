using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "TileOccupantDatabase", menuName = "ScriptableObjects/TileOccupantDatabase", order = 3)]
public class TileOccupantDatabase : ScriptableObject
{
    [Serializable] 
    public class TileOccupantConfig
    {
        public OccupantType occupantType;
        public GameObject occupantPrefab;
    }

    [SerializeField] private TileOccupantConfig[] configs;

    public GameObject GetPrefab(OccupantType type)
    {
        TileOccupantConfig foundConfig = configs.FirstOrDefault(item => item.occupantType == type);

        if (foundConfig == null) return null;

        return foundConfig.occupantPrefab;
    }
}
