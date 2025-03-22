using System.Collections.Generic;
using UnityEngine;

public class GridAction_StartGame : GridAction_ChangeScene
{
    protected override bool ActualRunAction(GridObject subject, Vector2Int position)
    {
        GameManager.Instance.Reset();

        return base.ActualRunAction(subject, position);
    }
}
