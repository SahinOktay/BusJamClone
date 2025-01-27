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
    [HideInInspector] public Tile up;
    [HideInInspector] public Tile right;
    [HideInInspector] public Tile down;
    [HideInInspector] public Tile left;

    public Action StatusChange;

    public BaseTileOccupier Occupier => tileOccupier;

    public void SetupVisuals()
    {
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

        TileVisualStatus visualConfig = visualConfigurations.FirstOrDefault(item =>
            item.isWalkableUp == (up != null ? up.isWalkable : false) &&
            item.isWalkableRight == (right != null ? right.isWalkable : false) &&
            item.isWalkableDown == (down != null ? down.isWalkable : true) &&
            item.isWalkableLeft == (left != null ? left.isWalkable : false)
        );

        if (visualConfig == null) return;

        for (int i = 0; i < visualConfig.objectsToActivate.Length; i++)
        {
            visualConfig.objectsToActivate[i].SetActive(true);
        }
    }

    public void DestroyOccupier(bool editorDestroy = false)
    {
        if (editorDestroy)
        {
            if (tileOccupier != null)
            {
                DestroyImmediate(tileOccupier.gameObject);
            }
            return;
        }
    }

    public void SetOccupier(BaseTileOccupier newOccupier)
    {
        occupantType = newOccupier.OccupantType;
        if (newOccupier == null) return;

        tileOccupier = newOccupier;
        newOccupier.transform.SetParent(tileOccupierParent);
        newOccupier.transform.localPosition = Vector3.zero;
    }
}
