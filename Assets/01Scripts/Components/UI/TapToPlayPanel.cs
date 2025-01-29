using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapToPlayPanel : Panel
{
    [SerializeField] private Transform playText;
    public override void ShowPanel()
    {
        base.ShowPanel();
        ScaleUp();
    }

    private void ScaleUp()
    {
        playText.DOScale(Vector3.one * 1.2f, .25f).SetEase(Ease.InOutCubic)
            .OnComplete(() => { ScaleDown(); });
    }

    private void ScaleDown()
    {
        playText.DOScale(Vector3.one, .25f).SetEase(Ease.InOutCubic)
            .OnComplete(() => { ScaleUp(); });
    }

    public override void HidePanel()
    {
        base.HidePanel();
        playText.DOKill();
    }
}
