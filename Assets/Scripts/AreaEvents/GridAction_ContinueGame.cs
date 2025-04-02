using System.Collections.Generic;
using UnityEngine;

public class GridAction_ContinueGame : GridActionContainer
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
        throw new System.NotImplementedException();
    }
}
