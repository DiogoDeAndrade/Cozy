using System.Collections.Generic;
using UnityEngine;

public class GridAction_StartGame : GridAction_ChangeScene
{
    protected override bool RunAction(GridObject subject, Vector2Int position)
    {
        GameManager.Instance.Reset();

        return base.RunAction(subject, position);
    }
}
