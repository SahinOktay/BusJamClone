using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTileOccupier : MonoBehaviour
{
    [SerializeField] private OccupantType occupantType;

    [NonSerialized] public Vector2Int coordinates;

    public Action<BaseTileOccupier> LeftTile;

    public OccupantType OccupantType => occupantType;
    public virtual void TakeAction() { }
}
