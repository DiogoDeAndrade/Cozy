using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    struct Action
    {
        public GridAction   action;
        public KeyCode      keyCode;
    }

    private GridSystem          gridSystem;
    private GridObject          gridObject;
    private List<Action>        availableActions;

    void Start()
    {
        gridSystem = GetComponentInParent<GridSystem>();
        gridObject = GetComponent<GridObject>();
    }

    void Update()
    {
        HandleOptions();
    }

    void HandleOptions()
    {
        var position = gridObject.GetPositionFacing();

        var actions = gridSystem.GetActions(gridObject, position);

        int cIndex = -1;

        availableActions = new();

        bool[] keys = new bool[26];
        foreach (var action in actions)
        {
            // Assign a key
            var verb = action.verb.ToUpper();
            char c = '\0';
            for (int i = 0; i < verb.Length; i++)
            {
                char cc = verb[i];
                cIndex = ((int)cc) - 'A';
                if (!keys[cIndex])
                {
                    c = cc;
                    break;
                }
            }
            if (c == '\0')
            {
                // Select the first unused letter
                for (int i = 0; i < keys.Length; i++)
                {
                    if (!keys[i])
                    {
                        c = (char)(i + 'A');
                        break;
                    }
                }
            }
            cIndex = ((int)c) - 'A';
            keys[cIndex] = true;
            availableActions.Add(new Action()
            {
                keyCode = (KeyCode)(KeyCode.A + cIndex),
                action = action
            });
        }
    }

    public List<string> GetAvailableOptions()
    {
        List<string> ret = new();
        if (availableActions != null)
        {
            foreach (var action in availableActions)
            {
                ret.Add($"({action.keyCode}): {action.action.verb}");
            }
        }

        return ret;
    }

    private void OnDrawGizmosSelected()
    {
        if (gridObject == null) return;

        var position = gridObject.GetPositionFacing();
        Gizmos.color = new Color(0.5f, 0.1f, 0.5f, 0.5f);
        Gizmos.DrawSphere(gridSystem.GridToWorld(position), 5.0f);
    }
}
