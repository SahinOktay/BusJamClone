using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinPanel : Panel
{
    [SerializeField] private Button continueButton;
    [SerializeField] private TMP_Text levelText;

    public Action ContinueClick;

    public void SetLevelText(int level)
    {
        levelText.text = "Level " + level + "\n Complete!";
    }

    public override void ShowPanel()
    {
        base.ShowPanel();
        continueButton.onClick.AddListener(OnContinueClick);
    }

    public override void HidePanel()
    {
        base.HidePanel();
        continueButton.onClick.RemoveListener(OnContinueClick);
    }

    private void OnContinueClick()
    {
        ContinueClick?.Invoke();
    }
}
