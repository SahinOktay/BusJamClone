using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class EditorSpawnManager : MonoBehaviour, ISpawnManager
{
    [SerializeField] private GameObject[] gameObjects;

    public T GetElement<T>() where T : MonoBehaviour
    {
        for (int i = 0; i < gameObjects.Length; i++)
        {
            T foundObject = gameObjects[i].GetComponent<T>();
            if (foundObject != null)
                return ((GameObject)PrefabUtility.InstantiatePrefab(gameObjects[i])).GetComponent<T>();
        }

        return null;
    }

    public void RecycleElement<T>(T element) where T : MonoBehaviour
    {
        DestroyImmediate(element.gameObject);
    }
}
