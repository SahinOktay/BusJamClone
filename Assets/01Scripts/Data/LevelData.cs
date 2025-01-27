using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Rollic;

[Serializable]
public class LevelData
{
    public int level_number;
    public bool[] tileStatus;
    public CharacterData[] characters;
    public GameLogicColor[] busColors;
    public TunnelData[] tunnels;

    public static LevelData GenerateNewData()
    {
        LevelData generatedData = new LevelData();

        generatedData.characters = new CharacterData[0];
        generatedData.tunnels = new TunnelData[0];
        generatedData.busColors = new GameLogicColor[0];
        int tileCount = Constants.GridDimentions.x * Constants.GridDimentions.y;
        generatedData.tileStatus = new bool[tileCount];

        for (int i = 0; i < tileCount; i++)
        {
            generatedData.tileStatus[i] = false;
        }

        return generatedData;
    }
}
