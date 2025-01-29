using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelComponents
{
    public Grid grid;
    public Character[] characters;
    public Tunnel[] tunnels;

    public LevelComponents(Vector2Int dimensios) 
    {
        grid = new Grid(dimensios);
        characters = new Character[0];
        tunnels = new Tunnel[0];
    }

    public void SetTile(int index, Tile tile)
    {
        grid.tiles[index] = tile;
    }
}
