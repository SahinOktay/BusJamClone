#if UNITY_EDITOR
using Rollic;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class LevelEditor : MonoBehaviour
{
    [SerializeField] private ColorDatabase colorDatabase;
    [SerializeField] private EditorSpawnManager spawnManager;
    [SerializeField] private SceneSetter sceneSetter;
    [SerializeField] private TileOccupantDatabase tileOccupantDatabase;
    [SerializeField] private LevelComponents levelComponents;

    public static bool Initialized {  get; private set; }
    public static ColorDatabase ColorDatabase {  get; private set; }

    public void Initialize()
    {
        ColorDatabase = colorDatabase;
        Initialized = true;
    }

    public void SetupLevel(LevelData levelData)
    {
        UnloadLevel();

        levelComponents = sceneSetter.SetupLevel(levelData, spawnManager);

        for (int i = 0; i < levelComponents.grid.tiles.Length; i++)
        {
            levelComponents.grid.tiles[i].StatusChange += OnTileStatusChange;
            levelComponents.grid.tiles[i].NeedNewOccupier += OnNewOccupantRequest;
        }
    }


    public void UnloadLevel()
    {
        levelComponents = sceneSetter.UnloadLevel(spawnManager);
    }

    public Vector2Int GetCoordinate(int index)
    {
        return new Vector2Int(index % levelComponents.grid.dimensions.x, index / levelComponents.grid.dimensions.x);
    }

    public LevelData GetLevelData()
    {
        LevelData newData = LevelData.GenerateNewData(levelComponents.grid.dimensions);
        List<CharacterData> characterDatas = new List<CharacterData>();
        List<TunnelData> tunnelDatas = new List<TunnelData>();

        CharacterData currentCharData;
        TunnelData currentTunnelData;
        for (int i = 0; i < levelComponents.grid.tiles.Length; i++)
        {
            newData.tileStatus[i] = levelComponents.grid.tiles[i].isWalkable;

            switch (levelComponents.grid.tiles[i].occupantType)
            {
                case OccupantType.Empty:
                    break;
                case OccupantType.Character:
                    currentCharData = (levelComponents.grid.tiles[i].Occupier as Character).GetCharacterData();
                    currentCharData.coordinates = GetCoordinate(i);
                    characterDatas.Add(currentCharData);
                    break;
                case OccupantType.Tunnel:
                    currentTunnelData = (levelComponents.grid.tiles[i].Occupier as Tunnel).GetTunnelData();
                    currentTunnelData.coordinates = GetCoordinate(i);
                    tunnelDatas.Add(currentTunnelData);
                    break;
                default: break;
            }
        }

        newData.characters = characterDatas.ToArray();
        newData.tunnels = tunnelDatas.ToArray();

        return newData;
    }

    private void OnTileStatusChange()
    {
        sceneSetter.SetTileVisuals();
    }

    private void OnNewOccupantRequest(Tile tile, OccupantType occupantType)
    {
        if (tile.Occupier != null)
        {
            spawnManager.RecycleElement(tile.Occupier);
            tile.ClearOccupier();
        }

        switch (occupantType)
        {
            case OccupantType.Empty:
                return;
            case OccupantType.Character:
                tile.SetOccupier(spawnManager.GetElement<Character>());
                return;
            case OccupantType.Tunnel:
                tile.SetOccupier(spawnManager.GetElement<Tunnel>());
                return;
            default: break;
        }
    }
}
#endif