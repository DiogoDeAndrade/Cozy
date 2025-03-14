using System.Collections.Generic;
using UnityEngine;

public class GridAction_ContinueGame : GridAction
{
    public override void GatherActions(GridObject subject, Vector2Int position, List<GridAction> actions)
    {
        actions.Add(this);
    }

    public override bool RunAction(GridObject subject, Vector2Int position)
    {
        throw new System.NotImplementedException();
    }
}
