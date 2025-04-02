using System.Collections.Generic;
using UnityEngine;

public class GridAction_Exit : GridActionContainer
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
        FullscreenFader.FadeOut(1.0f, Color.black, () =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else        
            Application.Quit();
               
#endif
        });

        return true;
    }
}
