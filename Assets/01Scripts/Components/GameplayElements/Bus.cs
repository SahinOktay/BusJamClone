using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Bus : MonoBehaviour
{
    [SerializeField] private GameObject[] seats;
    [SerializeField] private Renderer[] renderers;

    private int _reservedSeatCount = 0;
    private readonly List<Character> _satCharacters = new List<Character>();
    private GameLogicColor _color;

    public Action<Bus> BusArrived;
    public Action<Bus> BusFull;

    public GameLogicColor LogicColor => _color;

    public void Initialize(ColorDatabase.ColorConfig colorConfig)
    {
        _satCharacters.Clear();
        _color = colorConfig.gameLogicColor;
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material = colorConfig.busMat;
        }
    }

    public void Move(Vector3 pos, float startDelay = 0)
    {
        transform.DOMove(pos, 1f).SetEase(Ease.InOutCubic).SetDelay(startDelay)
            .OnComplete(() => { BusArrived?.Invoke(this); });
    }

    public void MoveOutOfScreenAndRecycle(ISpawnManager spawnManager)
    {
        transform.DOMove(transform.position + Vector3.right * 25, 1.5f).SetEase(Ease.InCubic)
            .OnComplete(
            () => 
            { 
                spawnManager.RecycleElement(this);
                for (int i = 0; i < _satCharacters.Count; i++)
                {
                    spawnManager.RecycleElement(_satCharacters[i]);
                }
            }
        );
    }

    public bool ReserveSeat(Character character)
    {
        if (_reservedSeatCount >= seats.Length) return false;

        character.MoveToBus(this);
        _reservedSeatCount++;
        return true;
    }

    public void OnCharacterReached(Character character)
    {
        _satCharacters.Add(character);
        character.transform.SetParent(seats[_satCharacters.Count - 1].transform);
        character.transform.localPosition = Vector3.zero;

        if (_satCharacters.Count == seats.Length)
        {
            BusFull?.Invoke(this);
        }
    }
}
