using Rollic;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SceneSetter : MonoBehaviour
{
    [SerializeField] private ColorDatabase colorDatabase;
    [SerializeField] private Transform levelParent;
    [SerializeField] private Transform fillerLeft;
    [SerializeField] private Transform fillerMiddle;
    [SerializeField] private Transform fillerRight;
    [SerializeField] private TileOccupantDatabase tileOccupantDatabase;

    [SerializeField] private LevelComponents levelComponents;

    public LevelComponents SetupLevel(LevelData levelData, ISpawnManager spawnManager)
    {
        levelComponents = new LevelComponents(levelData.dimensions);
        float tileSize = Constants.Numbers.TileSize;

        Vector2Int dimensions = levelData.dimensions;
        fillerLeft.transform.position = Vector3.left * (dimensions.x * tileSize * .5f);
        fillerRight.transform.position = Vector3.right * (dimensions.x * tileSize * .5f);
        fillerMiddle.transform.position = Vector3.back * (dimensions.y * tileSize);
        levelParent.transform.position = new Vector3(
            -dimensions.x * (.5f * tileSize) + tileSize * .5f,
            0,
            -tileSize * .5f
        );

        levelParent.transform.SetParent(transform);
        Tile spawnedTile;

        int tileIndex = 0;

        for (int y = 0; y < dimensions.y; y++)
        {
            for (int x = 0; x < dimensions.x; x++)
            {
                spawnedTile = spawnManager.GetElement<Tile>();
                spawnedTile.transform.SetParent(levelParent);
                spawnedTile.transform.localPosition = new Vector3(x * tileSize, 0, -y * tileSize);

                spawnedTile.isWalkable = levelData.tileStatus[tileIndex];
                levelComponents.SetTile(tileIndex, spawnedTile);
                tileIndex++;
            }
        }

        List<Character> spawnedCharacters = new List<Character>();
        Character spawnedCharacter;
        for (int i = 0; i < levelData.characters.Length; i++)
        {
            spawnedCharacter = SpawnCharacter(levelData.characters[i].color, spawnManager);
            levelComponents.grid.GetTile(levelData.characters[i].coordinates, out Tile tile);

            tile.SetOccupier(spawnedCharacter);
            spawnedCharacters.Add(spawnedCharacter);
            spawnedCharacter.coordinates = levelData.characters[i].coordinates;
        }

        List<Tunnel> spawnedTunnels = new List<Tunnel>();
        Tunnel currentTunel;
        Stack<Character> tunnelCharacters;
        for (int i = 0; i < levelData.tunnels.Length; i++)
        {
            tunnelCharacters = new Stack<Character>();
            for (int j = levelData.tunnels[i].characterColors.Length - 1; j > -1; j--)
            {
                spawnedCharacter = SpawnCharacter(levelData.tunnels[i].characterColors[j], spawnManager);
                tunnelCharacters.Push(spawnedCharacter);
                spawnedCharacters.Add(spawnedCharacter);
            }

            currentTunel = spawnManager.GetElement<Tunnel>();

            currentTunel.Setup(levelData.tunnels[i].direction, tunnelCharacters);
            spawnedTunnels.Add(currentTunel);

            levelComponents.grid.GetTile(levelData.tunnels[i].coordinates, out Tile tile);
            tile.SetOccupier(currentTunel);
        }

        levelComponents.tunnels = spawnedTunnels.ToArray();
        levelComponents.characters = spawnedCharacters.ToArray();

        SetTileVisuals();

        return levelComponents;
    }

    public void SetTileVisuals()
    {
        Tile currentTile;
        Tile neighborTile;
        bool up = false, right = false, down = true, left = false;
        for (int y = 0; y < levelComponents.grid.dimensions.y; y++)
        {
            for (int x = 0; x < levelComponents.grid.dimensions.x; x++)
            {
                levelComponents.grid.GetTile(new Vector2Int(x, y), out currentTile);

                if (levelComponents.grid.GetTile(new Vector2Int(x, y + 1), out neighborTile))
                {
                    up = neighborTile.isWalkable;
                }

                if (levelComponents.grid.GetTile(new Vector2Int(x + 1, y), out neighborTile))
                {
                    right = neighborTile.isWalkable;
                }

                if (levelComponents.grid.GetTile(new Vector2Int(x, y - 1), out neighborTile))
                {
                    down = neighborTile.isWalkable;
                }

                if (levelComponents.grid.GetTile(new Vector2Int(x - 1, y), out neighborTile))
                {
                    left = neighborTile.isWalkable;
                }

                currentTile.Initialize(up, right, down, left, new Vector2Int(x, y));
            }
        }
    }

    public LevelComponents UnloadLevel(ISpawnManager spawnManager)
    {
        for (int i = 0; i < levelComponents.characters.Length; i++)
        {
            if (levelComponents.characters[i] != null)
            {
                spawnManager.RecycleElement(levelComponents.characters[i]);
            }
        }

        for (int i = 0; i < levelComponents.tunnels.Length; i++)
        {
            if (levelComponents.tunnels[i] != null)
            {
                spawnManager.RecycleElement(levelComponents.tunnels[i]);
            }
        }

        for (int i = 0; i < levelComponents.grid.tiles.Length; i++)
        {
            if (levelComponents.grid.tiles[i] != null)
            {
                spawnManager.RecycleElement(levelComponents.grid.tiles[i]);
            }
        }

        // Reset level components
        levelComponents = new LevelComponents(levelComponents.grid.dimensions);
        return levelComponents;
    }

    public void UpdateTileOccupant()
    {

    }

    private Character SpawnCharacter(GameLogicColor color, ISpawnManager spawnManager)
    {
        Character newCharacter = spawnManager.GetElement<Character>();

        newCharacter.Setup(colorDatabase.GetColorConfig(color));

        return newCharacter;
    }
}
