
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public class Player : MonoBehaviour
{
    [System.Serializable]
    class ItemResource
    {
        public Item             item;
        public ResourceType     resType;
        public ResourceHandler  handler;
    }

    [SerializeField] private List<ItemResource> itemResource;
    [SerializeField] private Hypertag           tagPlayerSpawnPoint;
    [SerializeField] private ResourceHandler    lightLifeHandler;

    struct Action
    {
        public GridAction   action;
        public KeyCode      keyCode;
    }

    private GridSystem          gridSystem;
    private GridObject          gridObject;
    private MovementGridXY      movementGrid;
    private Inventory           inventory;
    private List<Action>        availableActions;
    private bool                actionsEnabled = true;
    private Transform           origin;

   
    void Start()
    {
        gridSystem = GetComponentInParent<GridSystem>();
        gridObject = GetComponent<GridObject>();
        movementGrid = GetComponent<MovementGridXY>();
        inventory = GetComponent<Inventory>();

        var inventoryDisplay = FindFirstObjectByType<InventoryDisplay>();
        if (inventoryDisplay != null)
        {
            inventoryDisplay.SetInventory(inventory);
        }

        foreach (var r in itemResource)
        {
            r.handler = this.FindResourceHandler(r.resType);
            r.handler.enabled = inventory.HasItem(r.item);
        }

        origin = Hypertag.FindFirstObjectWithHypertag<Transform>(tagPlayerSpawnPoint);
        transform.position = gridSystem.Snap(origin.transform.position);

        inventory.onChange += OnInventoryUpdate;
        lightLifeHandler.onResourceEmpty += TeleportToOrigin;
        gridObject.onMoveEnd += MoveEnd;
    }

    private void TeleportToOrigin(GameObject changeSource)
    {
        // Player ran out of life light
        origin = Hypertag.FindFirstObjectWithHypertag<Transform>(tagPlayerSpawnPoint);
        transform.position = gridSystem.Snap(origin.transform.position);
        lightLifeHandler.ResetResource();
    }

    private void MoveEnd(Vector2Int sourcePos, Vector2Int destPos)
    {
        StartCoroutine(RunActionsDelayCR(0.0f));
    }

    private void OnInventoryUpdate(bool add, Item item, int slot)
    {
        foreach (var r in itemResource)
        {
            if (r.item == item)
            {
                r.handler.enabled = add;
            }
        }
    }

    void Update()
    {
        HandleOptions();

        if (actionsEnabled)
        {
            foreach (var action in availableActions)
            {
                if (Input.GetKeyDown(action.keyCode))
                {
                    if (action.action.RunAction(gridObject, gridObject.GetPositionFacing()))
                    {
                        StartCoroutine(RunActionsDelayCR(0.5f));
                    }
                }
            }
        }
    }

    IEnumerator RunActionsDelayCR(float delayTime)
    {
        bool prevState = EnableActions(false);

        if (delayTime > 0)
            yield return new WaitForSeconds(delayTime);

        try
        {
            ITurnExecute.ExecuteAllTurns();
        }
        catch(Exception e)
        {
            EnableActions(prevState);
            Debug.LogError($"There was an exception running turns: {e.Message}");
            throw e;
        }

        EnableActions(prevState);
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

    public int GetFacing() => gridObject.GetFacingDirection();

    public bool EnableActions(bool b)
    {
        bool prevState = actionsEnabled;
        
        actionsEnabled = b;
        movementGrid.enabled = b;

        return prevState;
    }
}
