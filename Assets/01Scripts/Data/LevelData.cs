using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Rollic;
using System.Linq;

[Serializable]
public class LevelData
{
    public int level_number;
    public Vector2Int dimensions;
    public bool[] tileStatus;
    public CharacterData[] characters;
    public GameLogicColor[] busColors;
    public TunnelData[] tunnels;

    public static LevelData GenerateNewData(Vector2Int dimentions)
    {
        LevelData generatedData = new LevelData();

        generatedData.characters = new CharacterData[0];
        generatedData.tunnels = new TunnelData[0];
        generatedData.busColors = new GameLogicColor[0];
        generatedData.dimensions = dimentions;
        int tileCount = dimentions.x * dimentions.y;
        generatedData.tileStatus = new bool[tileCount];

        for (int i = 0; i < tileCount; i++)
        {
            generatedData.tileStatus[i] = false;
        }

        return generatedData;
    }

    public void Resize(Vector2Int newSize)
    {
        List<List<bool>> tileStatusAs2d = new List<List<bool>>();

        // Convert 1d array to 2d to make calculations simplier
        for (int y = 0; y < dimensions.y; y++)
        {
            tileStatusAs2d.Add(new List<bool>());
            for (int x = 0; x < dimensions.x; x++)
            {
                tileStatusAs2d[y].Add(tileStatus[y * dimensions.x + x]);
            }
        }

        // if new size y is bigger, add new rows
        while (tileStatusAs2d.Count < newSize.y)
        {
            tileStatusAs2d.Add(new List<bool>());
        }

        // if new size x is bigger, add new columns
        for (int y = 0; y < newSize.y; y++)
        {
            while (tileStatusAs2d[y].Count < newSize.x)
                tileStatusAs2d[y].Add(false);
        }

        // if new size y is smaller, remove old rows from bottom 
        while (tileStatusAs2d.Count > newSize.y)
        {
            tileStatusAs2d.RemoveAt(tileStatusAs2d.Count - 1);
        }

        // if new size x is smaller, remove old columns from right
        for (int y = 0; y < newSize.y; y++)
        {
            while (tileStatusAs2d[y].Count > newSize.x)
            {
                tileStatusAs2d[y].RemoveAt(tileStatusAs2d[y].Count - 1);
            }
        }

        // Convert to 1d array back
        tileStatus = new bool[newSize.x * newSize.y];

        for (int y = 0; y < newSize.y; y++)
        {
            for (int x = 0; x < newSize.x; x++)
            {
                tileStatus[y * newSize.x + x] = tileStatusAs2d[y][x];
            }
        }

        // Remove characters that are out of new dimentions
        List<CharacterData> characterList = characters.ToList();

        for (int i = 0; i < characterList.Count; i++)
        {
            if (
                characterList[i].coordinates.x >= newSize.x
                ||
                characterList[i].coordinates.y >= newSize.y
            )
            {
                characterList.RemoveAt(i--);
            }
        }

        characters = characterList.ToArray();

        // Remove tunnels that are out of new dimentions
        List<TunnelData> tunnelList = tunnels.ToList();

        for (int i = 0; i < tunnelList.Count; i++)
        {
            if (
                tunnelList[i].coordinates.x >= newSize.x
                ||
                tunnelList[i].coordinates.y >= newSize.y
            )
            {
                tunnelList.RemoveAt(i--);
            }
        }

        tunnels = tunnelList.ToArray();

        dimensions = newSize;
    }
}
