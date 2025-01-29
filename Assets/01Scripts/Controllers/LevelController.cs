using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Rollic;
using System.Linq;

public class LevelController : MonoBehaviour
{
    public static Vector2Int[] allDirections = new Vector2Int[] {
        Vector2Int.down, Vector2Int.right, Vector2Int.up, Vector2Int.left
    };

    [SerializeField] private ColorDatabase colorDatabase;
    [SerializeField] private InputController inputController;
    [SerializeField] private PoolManager poolManager;
    [SerializeField] private SceneSetter sceneSetter;
    [SerializeField] private Timer timer;
    [SerializeField] private Transform busEntrance;
    [SerializeField] private WaitingQueueController waitingQueueController;
    [SerializeField] private UIController uIController;

    private LevelData _levelData;
    private LevelComponents _levelComponents;
    private Vector3 _busEntrancePos;
    private readonly Queue<Bus> _busses = new Queue<Bus>();
    private Bus[] _allBusses;

    public Action LevelComplete, TryAgain, NextLevel;

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
                i * .25f
            );
        }
        _busses.Peek().BusFull += OnBusFull;
        _allBusses = _busses.ToArray();

        inputController.Tap += StartGame;
        uIController.ShowTapToPlay();
        uIController.ShowHud(_levelData.level_number, Mathf.FloorToInt(_levelData.duration));
    }

    private void StartGame()
    {
        inputController.Tap -= StartGame;
        SetMovableCharacters();
        uIController.HideTapToPlay();
        timer.CountDownComplete += OnCountDownEnded;
        timer.CountDown(_levelData.duration);
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

                    OptimizePath(path, next);

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

    private void OptimizePath(List<Vector2Int> path, Vector2Int newTile)
    {
        for (int i = 0; i < path.Count - 1; i++)
        {
            if (Mathf.Abs(path[i].x - newTile.x) + Mathf.Abs(path[i].y - newTile.y) == 1)
            {
                path.RemoveRange(i + 1, path.Count - (i + 1));
                break;
            }
        }
    }

    private void OnCountDownEnded()
    {
        uIController.ShowLoseScreen(_levelData.level_number);
        uIController.TryAgainClick += OnTryAgain;
    }

    private void OnBusArrived(Bus bus)
    {
        bus.BusArrived -= OnBusArrived;
        waitingQueueController.NewBusArrived(bus);
    }

    private void OnBusFull(Bus bus)
    {
        bus.BusFull -= OnBusFull;
        _busses.Dequeue().MoveOutOfScreen();

        if (_busses.Count == 0)
        {
            timer.Stop();
            LevelComplete?.Invoke();
            uIController.NextLevelClick += OnNextLevel;
            uIController.ShowWinScreen(_levelData.level_number);
            return;
        }

        int index = 0;
        foreach (Bus queueBus in _busses)
        {
            queueBus.Move(
                _busEntrancePos + Vector3.left * (index * Constants.Numbers.BusLength),
                (index + 1) * .5f
            );
            index++;
        }

        _busses.Peek().BusFull += OnBusFull;
        _busses.Peek().BusArrived += OnBusArrived;
    }

    private void OnCharacterOutOfGrid(Character character)
    {
        character.OutOfGrid -= OnCharacterOutOfGrid;
        bool foundPlace = false;
        if (character.LogicColor == _busses.Peek().LogicColor)
        {
            foundPlace = _busses.Peek().ReserveSeat(character);
            if (!foundPlace)
            {
                foundPlace = waitingQueueController.AddCharacter(character);
            }
        }
        else
        {
            foundPlace = waitingQueueController.AddCharacter(character);
        }

        if (!foundPlace)
        {
            timer.Stop();
            uIController.ShowLoseScreen(_levelData.level_number);
            uIController.TryAgainClick += OnTryAgain;
        }
    }

    private void OnNextLevel()
    {
        uIController.HideResultScreens();
        uIController.NextLevelClick -= OnNextLevel;
        NextLevel?.Invoke();
    }

    private void OnTryAgain()
    {
        uIController.HideResultScreens();
        uIController.TryAgainClick -= OnTryAgain;
        TryAgain?.Invoke();
    }

    private void TapOnCharacter(Character character)
    {
        List<Vector2Int> tilePath = FindPathToBus(character.coordinates);
        List<Vector3> worldPath = tilePath.Select(item => _levelComponents.grid.GetTile(item).transform.position).ToList();

        character.OutOfGrid -= OnCharacterOutOfGrid;
        character.OutOfGrid += OnCharacterOutOfGrid;
        character.WalkOutOfGrid(worldPath);

        SetMovableCharacters();
    }

    public void UnloadLevel()
    {
        for (int i = 0; i < _levelComponents.characters.Length; i++)
        {
            poolManager.RecycleElement(_levelComponents.characters[i]);
        }

        for (int i = 0; i < _levelComponents.tunnels.Length; i++)
        {
            poolManager.RecycleElement(_levelComponents.tunnels[i]);
        }

        for (int i = 0; i < _levelComponents.grid.tiles.Length; i++)
        {
            poolManager.RecycleElement(_levelComponents.grid.tiles[i]);
        }

        if (_allBusses != null)
        {
            for (int i = 0; i < _allBusses.Length; i++)
            {
                poolManager.RecycleElement(_allBusses[i]);
            }
        }
    }
}
