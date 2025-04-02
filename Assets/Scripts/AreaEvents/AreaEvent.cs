using System;
using System.Collections.Generic;
using UnityEngine;

public class AreaEvent : MonoBehaviour
{
    [SerializeField, TextArea] 
    private string      description;
    [SerializeField]
    private bool        allowRetrigger;
    [SerializeField]
    private List<GridActionCondition> conditions;
    [SerializeField]
    private GridActionContainer  action;

    UCExpression.IContext   context;
    GridObject              player;

    private void Start()
    {
        context = InterfaceHelpers.GetFirstInterfaceComponent<UCExpression.IContext>();
    }

    private void Update()
    {
        foreach (var condition in conditions)
        {
            if (!condition.CheckCondition(context)) return;
        }

        if (player == null)
        {
            var p = FindFirstObjectByType<Player>();
            if (p) player = p.GetComponent<GridObject>();
        }

        var actions = new List<GridActionContainer.NamedAction>();
        action.GatherActions(player, player.GetFacingDirection2i(), actions);

        foreach (var action in actions)
        {
            if (action.Run(player, player.GetFacingDirection2i()))
            {
                if (!allowRetrigger) enabled = false;
            }
        }
    }
}
