using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Rollic;
using System.Linq;

public class LevelController : MonoBehaviour
{
    private static Vector2Int[] allDirections = new Vector2Int[] {
        Vector2Int.down, Vector2Int.right, Vector2Int.up, Vector2Int.left
    };

    [SerializeField] private ColorDatabase colorDatabase;
    [SerializeField] private InputController inputController;
    [SerializeField] private PoolManager poolManager;
    [SerializeField] private SceneSetter sceneSetter;
    [SerializeField] private Transform busEntrance;
    [SerializeField] private WaitingQueueController waitingQueueController;
    [SerializeField] private UIController uIController;

    private bool _busArrived = true;
    private LevelData _levelData;
    private LevelComponents _levelComponents;
    private Vector3 _busEntrancePos;
    private readonly Queue<Bus> _busses = new Queue<Bus>();

    public Action LevelComplete, LevelFail, TryAgain, NextLevel;

    public void InitializeLevel(int level)
    {
        waitingQueueController.Initialize();

        _busEntrancePos = busEntrance.position;
        _levelData = JsonUtility.FromJson<LevelData>(
            Resources.Load<TextAsset>("Levels/level_" + level).text
        );

        _levelComponents = sceneSetter.SetupLevel(_levelData, poolManager);

        for (int i = 0; i < _levelComponents.characters.Length; i++)
        {
            _levelComponents.characters[i].PlayerInteracted += TapOnCharacter;
        }

        _busses.Clear();

        Bus spawnedBus;
        for (int i = 0; i < _levelData.busColors.Length; i++)
        {
            spawnedBus = poolManager.GetElement<Bus>();
            _busses.Enqueue(spawnedBus);
            spawnedBus.transform.position = _busEntrancePos + Vector3.left * ((i + 1) * Constants.Numbers.BusLength);
            spawnedBus.Initialize(colorDatabase.GetColorConfig(_levelData.busColors[i]));
            spawnedBus.Move(
                _busEntrancePos + Vector3.left * (i * Constants.Numbers.BusLength), 
                i * .5f
            );
        }

        inputController.Tap += StartGame;
        _busses.Peek().BusFull += OnBusFull;
        uIController.ShowTapToPlay();
        uIController.ShowHud(_levelData.level_number, Mathf.FloorToInt(_levelData.duration));
    }

    private void StartGame()
    {
        inputController.Tap -= StartGame;
        SetMovableCharacters();
        uIController.HideTapToPlay();
    }

    private void SetMovableCharacters()
    {
        HashSet<Tile> searchedTiles = new HashSet<Tile>();
        HashSet<Tile> emptyTiles = new HashSet<Tile>();
        HashSet<Character> movableCharacters = new HashSet<Character>();

        Tile currentFrontTile;
        for (int i = 0; i < _levelComponents.grid.dimensions.x; i++)
        {
            _levelComponents.grid.GetTile(new Vector2Int(i, 0), out currentFrontTile);
            if (currentFrontTile.occupantType == OccupantType.Character)
            {
                movableCharacters.Add(currentFrontTile.Occupier as Character);
            }

            SearchCoordinate(currentFrontTile, searchedTiles, emptyTiles);
        }

        Tile neighborTile;
        foreach (Tile emptyTile in emptyTiles)
        {
            for (int i = 0; i < allDirections.Length; i++)
            {
                if (_levelComponents.grid.GetTile(emptyTile.Coordinates + allDirections[i], out neighborTile))
                {
                    if (neighborTile.occupantType == OccupantType.Character)
                    {
                        movableCharacters.Add(neighborTile.Occupier as Character);
                    }
                }
            }
        }

        foreach (Character movableCharacter in movableCharacters)
        {
            movableCharacter.SetCharacterMovable();
        }
    }

    private void SearchCoordinate(Tile currentTile, HashSet<Tile> searchedTiles, HashSet<Tile> emptyTiles)
    {
        if (searchedTiles.Contains(currentTile))
        {
            return;
        }

        searchedTiles.Add(currentTile);

        if (
            !currentTile.isWalkable ||
            currentTile.occupantType != OccupantType.Empty
        )
        {
            return;
        }

        emptyTiles.Add(currentTile);

        Tile neighborTile;

        for (int i = 0; i < allDirections.Length; i++)
        { 
            if (_levelComponents.grid.GetTile(currentTile.Coordinates + allDirections[i], out neighborTile))
            {
                SearchCoordinate(neighborTile, searchedTiles, emptyTiles);
            }
        }
    }

    public List<Vector2Int> FindPathToBus(Vector2Int start)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int current = start;
        Tile nextTile;
        Vector2Int comeFrom = Vector2Int.zero;

        path.Add(current);

        while (current.y > 0)
        {
            bool foundNext = false;

            for (int i = 0; i < allDirections.Length; i++)
            {
                if (allDirections[i] == comeFrom) continue;

                Vector2Int next = current + allDirections[i];
                if (path.Contains(next))
                    continue;

                if (_levelComponents.grid.GetTile(next, out nextTile))
                {
                    if (!nextTile.isWalkable || nextTile.occupantType != OccupantType.Empty) continue;

                    comeFrom = -allDirections[i];
                    current = next;
                    path.Add(current);
                    foundNext = true;
                    break;
                }
            }

            if (!foundNext)
            {
                return new List<Vector2Int>();
            }
        }

        return path;
    }

    private void OnBusArrived(Bus bus)
    {
        bus.BusArrived -= OnBusArrived;
        waitingQueueController.NewBusArrived(bus);
        _busArrived = true;
    }

    private void OnBusFull(Bus bus)
    {
        bus.BusFull -= OnBusFull;
        _busses.Dequeue().MoveOutOfScreenAndRecycle(poolManager);
        _busArrived = false;

        if (_busses.Count == 0)
        {
            // TODO: WIN
            return;
        }

        int index = 0;
        foreach (Bus queueBus in _busses)
        {
            queueBus.Move(
                _busEntrancePos + Vector3.left * (index * Constants.Numbers.BusLength),
                (index + 1) * .5f
            );
        }

        _busses.Peek().BusFull += OnBusFull;
        _busses.Peek().BusArrived += OnBusArrived;
    }

    private void OnCharacterOutOfGrid(Character character)
    {
        character.OutOfGrid -= OnCharacterOutOfGrid;
        if (character.LogicColor == _busses.Peek().LogicColor)
        {
            if (!_busses.Peek().ReserveSeat(character))
            {
                waitingQueueController.AddCharacter(character);
            }
        }
        else
        {
            waitingQueueController.AddCharacter(character);
        }
    }


    private void TapOnCharacter(Character character)
    {
        if (!_busArrived) return;

        List<Vector2Int> tilePath = FindPathToBus(character.coordinates);
        List<Vector3> worldPath = tilePath.Select(item => _levelComponents.grid.GetTile(item).transform.position).ToList();

        character.OutOfGrid += OnCharacterOutOfGrid;
        character.WalkOutOfGrid(worldPath);

        SetMovableCharacters();
    }

    public void UnloadLevel()
    {

    }
}
