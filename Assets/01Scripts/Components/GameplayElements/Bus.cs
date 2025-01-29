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
        _reservedSeatCount = 0;
        BusArrived = null;
        BusFull = null;
        _satCharacters.Clear();
        _color = colorConfig.gameLogicColor;
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material = colorConfig.busMat;
        }
    }

    public void Move(Vector3 pos, float startDelay = 0)
    {
        transform.DOMove(pos, .25f).SetEase(Ease.InOutCubic).SetDelay(startDelay)
            .OnComplete(() => { BusArrived?.Invoke(this); });
    }

    public void MoveOutOfScreen()
    {
        transform.DOMove(transform.position + Vector3.right * 25, .5f).SetEase(Ease.InCubic)
            .OnComplete(
            () => 
            { 
                gameObject.SetActive(false);
            }
        );
    }

    public bool ReserveSeat(Character character)
    {
        if (_reservedSeatCount >= seats.Length) return false;

        character.MoveToBus(this, new Vector3(0, 0, transform.position.z));
        _reservedSeatCount++;
        return true;
    }

    public void OnCharacterReached(Character character)
    {
        _satCharacters.Add(character);
        character.transform.SetParent(seats[_satCharacters.Count - 1].transform);
        character.transform.localPosition = Vector3.zero;
        character.transform.forward = character.transform.parent.forward;

        if (_satCharacters.Count == seats.Length)
        {
            BusFull?.Invoke(this);
        }
    }
}
