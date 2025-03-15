using System.Collections.Generic;
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

    private void Update()
    {
        foreach (var condition in conditions)
        {
            if (!condition.CheckCondition()) return;
        }

        if (action.RunAction(null, Vector2Int.zero))
        {
            if (!allowRetrigger) enabled = false;
        }
    }
}
