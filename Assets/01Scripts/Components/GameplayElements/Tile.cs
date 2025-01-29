using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [Serializable]
    public class TileVisualStatus
    {
        public bool isWalkableUp;
        public bool isWalkableRight;
        public bool isWalkableDown;
        public bool isWalkableLeft;

        public GameObject[] objectsToActivate;
    }

    public bool isWalkable = false;
    public OccupantType occupantType;

    [SerializeField] private GameObject walkable;
    [SerializeField] private GameObject unwalkable;
    [SerializeField] private TileVisualStatus[] visualConfigurations;
    [SerializeField] private Transform tileOccupierParent;

    [HideInInspector, SerializeField] private BaseTileOccupier tileOccupier;

    public Action StatusChange;
    public Action<Tile, OccupantType> NeedNewOccupier;

    public BaseTileOccupier Occupier => tileOccupier;
    public Vector2Int Coordinates { get; private set; }

    public void Initialize(bool up, bool right, bool down, bool left, Vector2Int coordinates)
    {
        Coordinates = coordinates;
        walkable.SetActive(isWalkable);
        unwalkable.SetActive(!isWalkable);
        if (isWalkable) return;

        for (int i = 0; i < visualConfigurations.Length; i++)
        {
            for (int j = 0; j < visualConfigurations[i].objectsToActivate.Length; j++)
            {
                visualConfigurations[i].objectsToActivate[j].SetActive(false);
            }
        }

        TileVisualStatus visualConfig = visualConfigurations.FirstOrDefault(
            item => item.isWalkableUp == up &&
                item.isWalkableRight == right &&
                item.isWalkableDown == down &&
                item.isWalkableLeft == left
        );

        if (visualConfig == null) return;

        for (int i = 0; i < visualConfig.objectsToActivate.Length; i++)
        {
            visualConfig.objectsToActivate[i].SetActive(true);
        }
    }

    public void ClearOccupier()
    {
        tileOccupier = null;
    }

    public void SetOccupier(BaseTileOccupier newOccupier)
    {
        occupantType = newOccupier.OccupantType;
        if (newOccupier == null) return;

        tileOccupier = newOccupier;
        tileOccupier.LeftTile += OnOccupierLeft;
        newOccupier.transform.SetParent(tileOccupierParent);
        newOccupier.transform.localPosition = Vector3.zero;
    }

    private void OnOccupierLeft(BaseTileOccupier occupier)
    {
        occupier.LeftTile -= OnOccupierLeft;
        occupantType = OccupantType.Empty;
    }
}
