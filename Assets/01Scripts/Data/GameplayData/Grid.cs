using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Grid
{
    public Vector2Int dimensions;
    public Tile[] tiles;

    public Grid(Vector2Int dimensions)
    {
        this.dimensions = dimensions;
        tiles = new Tile[dimensions.x * dimensions.y];
    }

    public Tile GetTile(Vector2Int vector) => tiles[vector.y * dimensions.x + vector.x];


    public bool GetTile(Vector2Int vector, out Tile tile)
    {
        if (
            vector.x >= dimensions.x ||
            vector.x < 0 || 
            vector.y >= dimensions.y ||
            vector.y < 0
        )
        {
            tile = null;
            return false;
        }

        tile = tiles[vector.y * dimensions.x + vector.x];
        return true;
    }

    public Vector2Int GetCoordinate(int index)
    {
        return new Vector2Int(index % dimensions.x, index / dimensions.x);
    }
}
