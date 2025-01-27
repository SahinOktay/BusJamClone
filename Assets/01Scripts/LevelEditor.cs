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
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Tile[] tiles;
    [SerializeField] private TileOccupantDatabase tileOccupantDatabase;


    public static bool Initialized {  get; private set; }
    public static ColorDatabase ColorDatabase {  get; private set; }
    public static TileOccupantDatabase TileOccupantDatabase {  get; private set; }

    private const string _levelParentName = "LevelParent";

    public void Initialize()
    {
        TileOccupantDatabase = tileOccupantDatabase;
        ColorDatabase = colorDatabase;
        Initialized = true;
    }

    public void SetupLevel(LevelData levelData)
    {
        UnloadLevel();

        Transform levelParent = new GameObject(_levelParentName).transform;
        levelParent.transform.position = Constants.GridStartPoint;
        levelParent.transform.SetParent(transform);
        Tile spawnedTile;

        tiles = new Tile[Constants.GridDimentions.x * Constants.GridDimentions.y];
        int tileIndex;

        for (int y = 0; y < Constants.GridDimentions.y; y++)
        {
            for (int x = 0; x < Constants.GridDimentions.x; x++)
            {
                spawnedTile = ((GameObject)PrefabUtility.InstantiatePrefab(tilePrefab)).GetComponent<Tile>();
                spawnedTile.transform.SetParent(levelParent);
                spawnedTile.transform.localPosition = new Vector3(x * Constants.TileSize, 0, y * Constants.TileSize);

                tileIndex = y * Constants.GridDimentions.x + x;
                spawnedTile.isWalkable = levelData.tileStatus[tileIndex];
                tiles[tileIndex] = spawnedTile;

                spawnedTile.StatusChange += OnTileStatusChange;
            }
        }

        Character currentCharacter;
        for (int i = 0; i < levelData.characters.Length; i++)
        {
            GetTile(levelData.characters[i].coordinates).SetOccupier(SpawnCharacter(levelData.characters[i].color));
        }

        Tunnel currentTunel;
        Stack<Character> tunnelCharacters;
        for (int i = 0; i < levelData.tunnels.Length; i++)
        {
            tunnelCharacters = new Stack<Character>();
            for (int j = levelData.tunnels[i].characterColors.Length - 1; j > -1; j--)
            {
                tunnelCharacters.Push(SpawnCharacter(levelData.tunnels[i].characterColors[j]));
            }

            currentTunel =
                ((GameObject)PrefabUtility.InstantiatePrefab(tileOccupantDatabase.GetPrefab(OccupantType.Tunnel)))
                .GetComponent<Tunnel>();

            currentTunel.Setup(levelData.tunnels[i].direction, tunnelCharacters);

            GetTile(levelData.tunnels[i].coordinates).SetOccupier(currentTunel);
        }

        Tile currentTile;
        for (int y = 0; y < Constants.GridDimentions.y; y++)
        {
            for (int x = 0; x < Constants.GridDimentions.x; x++)
            {
                currentTile = GetTile(x, y);
                if (y < Constants.GridDimentions.y - 1)
                {
                    currentTile.up = GetTile(x, y + 1);
                }

                if (x < Constants.GridDimentions.x - 1)
                {
                    currentTile.right = GetTile(x + 1, y);
                }

                if (y > 0)
                {
                    currentTile.down = GetTile(x, y - 1);
                }

                if (x > 0)
                {
                    currentTile.left = GetTile(x - 1, y);
                }

                currentTile.SetupVisuals();
            }
        }
    }

    private Character SpawnCharacter(GameLogicColor color)
    {
        Character newCharacter =
                ((GameObject)PrefabUtility.InstantiatePrefab(tileOccupantDatabase.GetPrefab(OccupantType.Character)))
                .GetComponent<Character>();

        newCharacter.Setup(colorDatabase.GetColorConfig(color));

        return newCharacter;
    }

    public void UnloadLevel()
    {
        Transform levelParent = transform.Find(_levelParentName);
        if (levelParent != null)
        {
            DestroyImmediate(levelParent.gameObject);
        }
    }

    public Tile GetTile(int x, int y)
    {
        return tiles[y * Constants.GridDimentions.x + x];
    }

    public Tile GetTile(Vector2Int vector)
    {
        return tiles[vector.y * Constants.GridDimentions.x + vector.x];
    }

    public Vector2Int GetCoordinate(int index)
    {
        return new Vector2Int(index % Constants.GridDimentions.x, index / Constants.GridDimentions.x);
    }

    public LevelData GetLevelData()
    {
        LevelData newData = LevelData.GenerateNewData();
        List<CharacterData> characterDatas = new List<CharacterData>();
        List<TunnelData> tunnelDatas = new List<TunnelData>();

        CharacterData currentCharData;
        TunnelData currentTunnelData;
        for (int i = 0; i < tiles.Length; i++)
        {
            newData.tileStatus[i] = tiles[i].isWalkable;

            switch (tiles[i].occupantType)
            {
                case OccupantType.Empty:
                    break;
                case OccupantType.Character:
                    currentCharData = (tiles[i].Occupier as Character).GetCharacterData();
                    currentCharData.coordinates = GetCoordinate(i);
                    characterDatas.Add(currentCharData);
                    break;
                case OccupantType.Tunnel:
                    currentTunnelData = (tiles[i].Occupier as Tunnel).GetTunnelData();
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
        for (int y = 0; y < Constants.GridDimentions.y; y++)
        {
            for (int x = 0; x < Constants.GridDimentions.x; x++)
            {
                GetTile(x, y).SetupVisuals();
            }
        }
    }
}
#endif