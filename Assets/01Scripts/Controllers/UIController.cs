using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private HudPanel hud;
    [SerializeField] private TapToPlayPanel tapToPlayPanel;
    [SerializeField] private WinPanel winPanel;
    [SerializeField] private LosePanel losePanel;

    public Action NextLevelClick;
    public Action TryAgainClick;

    public void Initialize()
    {
        winPanel.ContinueClick += TriggerNextLevel;
        losePanel.RetryClick += TriggerTryAgain;
    }

    public void ShowTapToPlay()
    {
        tapToPlayPanel.ShowPanel();
    }

    public void HideTapToPlay()
    {
        tapToPlayPanel.HidePanel();
    }

    public void ShowHud(int level, int seconds)
    {
        hud.SetLevelText(level);
        hud.SetTimer(seconds);
        hud.ShowPanel();
    }

    public void ShowWinScreen(int level)
    {
        hud.HidePanel();
        winPanel.SetLevelText(level);
        winPanel.ShowPanel();
    }

    public void ShowLoseScreen(int level)
    {
        hud.HidePanel();
        losePanel.SetLevelText(level);
        losePanel.ShowPanel();
    }

    public void HideResultScreens()
    {
        losePanel.HidePanel();
        winPanel.HidePanel();
    }

    public void UpdateTimer(int seconds)
    {
        hud.SetTimer(seconds);
    }

    private void TriggerNextLevel() => NextLevelClick?.Invoke();
    private void TriggerTryAgain() => TryAgainClick?.Invoke();
}
