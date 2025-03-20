using System.Collections.Generic;
using UnityEngine;

public class GridAction_Exit : GridAction
{
    protected override void ActualGatherActions(GridObject subject, Vector2Int position, List<GridAction> actions)
    {
        actions.Add(this);
    }

    protected override bool ActualRunAction(GridObject subject, Vector2Int position)
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
