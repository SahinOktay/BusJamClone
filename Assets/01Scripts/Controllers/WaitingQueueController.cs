using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingQueueController : MonoBehaviour
{
    [SerializeField] private List<WaitingSpot> spots;
    private List<Character> _waitingCharacters = new List<Character>();

    public void Initialize()
    {
        _waitingCharacters.Clear();
    }

    public bool AddCharacter(Character character)
    {
        if (_waitingCharacters.Count >= spots.Count) return false;

        _waitingCharacters.Add(character);
        character.MoveToWaitingSpot(spots[_waitingCharacters.Count - 1]);
        return true;
    }

    public void NewBusArrived(Bus bus)
    {
        for (int i = _waitingCharacters.Count - 1; i >= 0; i--)
        {
            if (_waitingCharacters[i].LogicColor == bus.LogicColor)
            {
                if (bus.ReserveSeat(_waitingCharacters[i]))
                {
                    _waitingCharacters.RemoveAt(i);
                }
            }
        }

        for (int i = 0; i < _waitingCharacters.Count; i++)
        {
            _waitingCharacters[i].MoveToWaitingSpot(spots[i]);
        }
    }
}
