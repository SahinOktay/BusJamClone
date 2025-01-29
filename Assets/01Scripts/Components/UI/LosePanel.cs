using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class LosePanel : Panel
{
    [SerializeField] private Button retryButton;
    [SerializeField] private TMP_Text levelText;

    public Action RetryClick;

    public void SetLevelText(int level)
    {
        levelText.text = "Level " + level + "\n Complete!";
    }

    public override void ShowPanel()
    {
        base.ShowPanel();
        retryButton.onClick.AddListener(OnContinueClick);
    }

    public override void HidePanel()
    {
        base.HidePanel();
        retryButton.onClick.RemoveListener(OnContinueClick);
    }

    private void OnContinueClick()
    {
        RetryClick?.Invoke();
    }
}
