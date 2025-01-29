using DG.Tweening;
using Rollic;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Character : BaseTileOccupier, IInteractable
{
    [SerializeField] private Animator characterAnimator;
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
        transform.forward = Vector3.forward;
        OutOfGrid = null;
        PlayerInteracted = null;

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

    public void Move(Vector3 pos)
    {
        characterAnimator.Play(Constants.Animations.Run, 0, 0);
        transform.DOMove(
            pos,
            Vector3.Distance(pos, transform.position) / Constants.Numbers.CharacterSpeed
        ).SetEase(Ease.Linear).OnComplete(() => { characterAnimator.Play(Constants.Animations.Idle, 0, 0); });
    }

    public void MoveToWaitingSpot(WaitingSpot spot)
    {
        characterAnimator.Play(Constants.Animations.Run, 0, 0);
        Vector3 spotPos = spot.transform.position;
        transform.forward = spotPos - transform.position;
        transform.DOMove(
            spotPos,
            Vector3.Distance(spotPos, transform.position) / Constants.Numbers.CharacterSpeed
        ).SetEase(Ease.Linear).OnComplete(() => {
            transform.forward = Vector3.forward;
            characterAnimator.Play(Constants.Animations.Idle, 0, 0); 
        });
    }

    public void MoveToBus(Bus bus, Vector3 pos)
    {
        transform.forward = pos - transform.position;
        characterAnimator.Play(Constants.Animations.Run, 0, 0);
        transform.DOMove(
            pos, 
            Vector3.Distance(pos, transform.position) / Constants.Numbers.CharacterSpeed
        ).SetEase(Ease.Linear).OnComplete(() => { 
            bus.OnCharacterReached(this);
            characterAnimator.Play(Constants.Animations.Sit, 0, 0);
        });
    }

    public void WalkOutOfGrid(List<Vector3> positions)
    {
        mCollider.enabled = false;
        mainRenderer.material = _currentColorConfig.passiveCharMat;

        float timeBetweenTiles = 0f;
        float delay = 0;

        LeftTile?.Invoke(this);

        if (positions.Count <= 1)
        {
            OutOfGrid?.Invoke(this);
            return;
        }

        characterAnimator.Play(Constants.Animations.Run, 0, 0);
        for (int i = 1; i < positions.Count; i++)
        {
            timeBetweenTiles = Vector3.Distance(positions[i], positions[i - 1]) / Constants.Numbers.CharacterSpeed;

            var tween = transform.DOMove(positions[i], timeBetweenTiles)
                .SetEase(Ease.Linear).SetDelay(delay).From(positions[i - 1]);

            tween.OnUpdate(() =>
            {
                transform.forward = tween.endValue - transform.position;
            });

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
