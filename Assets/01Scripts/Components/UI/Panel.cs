using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Panel : MonoBehaviour
{
    [SerializeField] private Canvas mCanvas;
    public virtual void ShowPanel()
    {
        mCanvas.enabled = true;
    }

    public virtual void HidePanel()
    {
        mCanvas.enabled = false;
    }
}
