using System.Collections.Generic;
using UnityEngine;

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

    struct Action
    {
        public GridAction   action;
        public KeyCode      keyCode;
    }

    private GridSystem          gridSystem;
    private GridObject          gridObject;
    private Inventory           inventory;
    private List<Action>        availableActions;
    

    void Start()
    {
        gridSystem = GetComponentInParent<GridSystem>();
        gridObject = GetComponent<GridObject>();
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

        inventory.onChange += OnInventoryUpdate;
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

        foreach (var action in availableActions)
        {
            if (Input.GetKeyDown(action.keyCode))
            {
                action.action.RunAction(gridObject, gridObject.GetPositionFacing());
            }
        }
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
}
