using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject characterPrefab;
    [SerializeField] private GameObject tunnelPrefab;

    private ObjectPool<Character> _characterPool;
    private ObjectPool<Tile> _tilePool;
    private ObjectPool<Tunnel> _tunnelPool;

    public void Initialize()
    {
        _characterPool = new ObjectPool<Character>(() => Instantiate(characterPrefab).GetComponent<Character>(), 10);
        _tunnelPool = new ObjectPool<Tunnel>(() => Instantiate(characterPrefab).GetComponent<Tunnel>(), 3);
        _tilePool = new ObjectPool<Tile>(() => Instantiate(characterPrefab).GetComponent<Tile>(), 30);
    }

    public Character GetCharacter()
    {
        Character character = _characterPool.Get();
        return character;
    }

    public void RecycleCustomer(Character character)
    {
        _characterPool.Return(character);
    }

    public Tile GetTile()
    {
        Tile tile = _tilePool.Get();
        return tile;
    }

    public void RecycleTile(Tile tile)
    {
        _tilePool.Return(tile);
    }

    public Tunnel GetTunnel()
    {
        Tunnel tunnel = _tunnelPool.Get();
        return tunnel;
    }

    public void RecycleTunnel(Tunnel tunnel)
    {
        _tunnelPool.Return(tunnel);
    }
}
