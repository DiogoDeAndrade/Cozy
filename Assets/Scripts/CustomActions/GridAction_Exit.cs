using System.Collections.Generic;
using UnityEngine;

public class GridAction_Exit : GridAction
{
    public override void GatherActions(GridObject subject, Vector2Int position, List<GridAction> actions)
    {
        actions.Add(this);
    }

    public override bool RunAction(GridObject subject, Vector2Int position)
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
