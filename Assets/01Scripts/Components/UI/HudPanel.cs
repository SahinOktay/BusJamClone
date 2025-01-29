using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HudPanel : Panel
{
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text timerText;

    public void SetLevelText(int level)
    {
        levelText.text = "LEVEL " + level;
    }

    public void SetTimer(int seconds)
    {
        timerText.text = seconds.ToString() + 's';
    }
}
