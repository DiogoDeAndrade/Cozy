using System;
using System.Collections.Generic;
using UnityEngine;

public class GridAction_Credits: GridActionContainer
{
    public override void ActualGatherActions(GridObject subject, Vector2Int position, List<NamedAction> retActions)
    {
        retActions.Add(new NamedAction
        {
            name = verb,
            action = RunAction,
            container = this
        });
    }

    protected bool RunAction(GridObject subject, Vector2Int position)
    {
        var player = FindAnyObjectByType<Player>();
        player.PushEnableAction(false);
        var creditsScroll = FindAnyObjectByType<BigTextScroll>();

        var canvasGroup = creditsScroll.GetComponentInParent<CanvasGroup>();
        canvasGroup.FadeIn(0.5f);

        creditsScroll.Reset();

        creditsScroll.onEndScroll += BackToMenu;

        return true;
    }

    private void BackToMenu()
    {
        var creditsScroll = FindAnyObjectByType<BigTextScroll>();

        var canvasGroup = creditsScroll.GetComponentInParent<CanvasGroup>();
        canvasGroup.FadeOut(0.5f);

        var player = FindAnyObjectByType<Player>();
        player.PopEnableAction();

        creditsScroll.onEndScroll -= BackToMenu;
    }
}
