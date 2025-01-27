using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTileOccupier : MonoBehaviour
{
    [SerializeField] private OccupantType occupantType;

    public OccupantType OccupantType => occupantType;
    public virtual void TakeAction() { }
}
