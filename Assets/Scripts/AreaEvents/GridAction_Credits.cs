using System;
using System.Collections.Generic;
using UnityEngine;

public class GridAction_Credits: GridAction
{
    bool prevActionState;

    protected override void ActualGatherActions(GridObject subject, Vector2Int position, List<GridAction> actions)
    {
        actions.Add(this);
    }

    protected override bool ActualRunAction(GridObject subject, Vector2Int position)
    {
        var player = FindAnyObjectByType<Player>();
        prevActionState = player.EnableActions(false);
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
        player.EnableActions(prevActionState);
    }
}
