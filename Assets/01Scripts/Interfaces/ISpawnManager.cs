using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpawnManager
{
    public T GetElement<T>() where T : MonoBehaviour;
    public void RecycleElement<T>(T element) where T : MonoBehaviour;
}
