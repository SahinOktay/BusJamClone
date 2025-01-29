using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PoolManager : MonoBehaviour, ISpawnManager
{
    [SerializeField] private GameObject busPrefab;
    [SerializeField] private GameObject characterPrefab;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject tunnelPrefab;

    private readonly Dictionary<Type, object> _poolMap = new Dictionary<Type, object>();

    public void Initialize()
    {
        ObjectPool<Character>  characterPool = new ObjectPool<Character>(() => Instantiate(characterPrefab).GetComponent<Character>(), 10);
        ObjectPool<Tunnel> tunnelPool = new ObjectPool<Tunnel>(() => Instantiate(tunnelPrefab).GetComponent<Tunnel>(), 3);
        ObjectPool<Tile> tilePool = new ObjectPool<Tile>(() => Instantiate(tilePrefab).GetComponent<Tile>(), 30);
        ObjectPool<Bus> busPool = new ObjectPool<Bus>(() => Instantiate(busPrefab).GetComponent<Bus>(), 5);

        _poolMap.Add(typeof(Character), characterPool);
        _poolMap.Add(typeof(Tunnel), tunnelPool);
        _poolMap.Add(typeof(Tile), tilePool);
        _poolMap.Add(typeof(Bus), busPool);
    }

    public T GetElement<T>() where T : MonoBehaviour
    {
        return (_poolMap[typeof(T)] as ObjectPool<T>).Get();
    }

    public void RecycleElement<T>(T element) where T : MonoBehaviour
    {
        element.gameObject.SetActive(false);
        element.transform.SetParent(null);
        (_poolMap[typeof(T)] as ObjectPool<T>).Return(element);
    }
}
