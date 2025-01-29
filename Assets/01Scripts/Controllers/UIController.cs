using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private TapToPlayPanel tapToPlayPanel;
    [SerializeField] private HudPanel hud;

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

    public void UpdateTimer(int seconds)
    {
        hud.SetTimer(seconds);
    }
}
