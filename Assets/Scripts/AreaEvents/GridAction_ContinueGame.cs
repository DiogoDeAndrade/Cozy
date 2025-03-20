using System.Collections.Generic;
using UnityEngine;

public class GridAction_ContinueGame : GridAction
{
    protected override void ActualGatherActions(GridObject subject, Vector2Int position, List<GridAction> actions)
    {
        actions.Add(this);
    }

    protected override bool ActualRunAction(GridObject subject, Vector2Int position)
    {
        throw new System.NotImplementedException();
    }
}
