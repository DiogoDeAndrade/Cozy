using System.Collections.Generic;
using UnityEditor.TerrainTools;
using UnityEngine;

public class AreaEvent : MonoBehaviour
{
    [SerializeField, TextArea] 
    private string      description;
    [SerializeField]
    private bool        allowRetrigger;
    [SerializeField]
    private List<AreaEventCondition> conditions;
    [SerializeField]
    private GridAction  action;

    UCExpression.IContext context;

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

        if (action.RunAction(null, Vector2Int.zero))
        {
            if (!allowRetrigger) enabled = false;
        }
    }
}
