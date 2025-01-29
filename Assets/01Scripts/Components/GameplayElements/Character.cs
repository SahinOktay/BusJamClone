using DG.Tweening;
using Rollic;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : BaseTileOccupier, IInteractable
{
    [SerializeField] private Collider mCollider;
    [SerializeField] private GameLogicColor color;
    [SerializeField] private Renderer mainRenderer;

    private ColorDatabase.ColorConfig _currentColorConfig;

    public Action<Character> PlayerInteracted;
    public Action<Character> OutOfGrid;

    public CharacterData GetCharacterData() => new CharacterData() { color = this.color };
    public GameLogicColor LogicColor => color;

    public void Setup(ColorDatabase.ColorConfig colorConfig)
    {
        color = colorConfig.gameLogicColor;
        _currentColorConfig = colorConfig;

        mainRenderer.material = _currentColorConfig.passiveCharMat;
    }

    public void SetCharacterMovable()
    {
        mainRenderer.material = _currentColorConfig.activeCharMat;
        mCollider.enabled = true;
    }

    public void GetInteracted()
    {
        PlayerInteracted?.Invoke(this);
    }

    public void ResetObject()
    {
        mCollider.enabled = false;
    }

    public void MoveToWaitingSpot(WaitingSpot spot)
    {
        Vector3 spotPos = spot.transform.position;
        transform.DOMove(
            spotPos,
            Vector3.Distance(spotPos, transform.position) / Constants.Numbers.CharacterSpeed
        ).SetEase(Ease.Linear);
    }

    public void MoveToBus(Bus bus)
    {
        Vector3 busPos = bus.transform.position;
        transform.DOMove(
            busPos, 
            Vector3.Distance(busPos, transform.position) / Constants.Numbers.CharacterSpeed
        ).SetEase(Ease.Linear).OnComplete(() => { bus.OnCharacterReached(this); });
    }

    public void WalkOutOfGrid(List<Vector3> positions)
    {
        mCollider.enabled = false;

        float timeBetweenTiles = 0f;
        float delay = 0;

        LeftTile?.Invoke(this);

        if (positions.Count <= 1)
        {
            OutOfGrid?.Invoke(this);
            return;
        }

        for (int i = 1; i < positions.Count; i++)
        {
            timeBetweenTiles = Vector3.Distance(positions[i], positions[i - 1]) / Constants.Numbers.CharacterSpeed;

            var tween = transform.DOMove(positions[i], timeBetweenTiles)
                .SetEase(Ease.Linear).SetDelay(delay).From(positions[i - 1]);

            delay += timeBetweenTiles;

            if (i == positions.Count - 1)
            {
                tween.OnComplete(() => OutOfGrid?.Invoke(this));
            }
        }
    }

#if UNITY_EDITOR
    public void OnValidate()
    {
        if (LevelEditor.Initialized)
            Setup(LevelEditor.ColorDatabase.GetColorConfig(this.color));
    }
#endif
}
