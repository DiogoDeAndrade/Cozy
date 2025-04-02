using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Plant : GridActionContainer
{
    [SerializeField, Header("Cut Tree")]
    private Item            cutTool;
    [SerializeField, PrefabParam]
    private ItemSeed        seedType;
    [SerializeField]
    private ItemPickup      itemPickupPrefab;

    protected override void Start()
    {
        base.Start();

        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer)
        {
            spriteRenderer.sprite = seedType.plantSprite;
            spriteRenderer.color = seedType.plantSpriteColor;
        }
    }

    bool HasTool(Inventory inventory, Item tool)
    {
        if (tool == null) return true;
        if (inventory == null) return false;
        return inventory.HasItem(tool);
    }

    public override void ActualGatherActions(GridObject subject, Vector2Int position, List<NamedAction> retActions)
    {
        var inventory = subject.GetComponent<Inventory>();

        if (HasTool(inventory, cutTool))
        {
            retActions.Add(new NamedAction
            {
                name = verb,
                action = CutPlant,
                container = this
            });
        }
    }

    protected bool CutPlant(GridObject subject, Vector2Int position)
    {
        if (seedType)
        {
            if (itemPickupPrefab)
            {
                // Find empty around this position
                if (gridSystem.FindMoore(2, position, FindEmptyCriteria, out var gridObj, out var pos))
                {
                    var newItem = Instantiate(itemPickupPrefab, gridSystem.GridToWorld(pos), Quaternion.identity);
                    newItem.SetItem(seedType);
                }
            }
            else
            {
                var inventory = subject.GetComponent<Inventory>();
                if (!inventory.Add(seedType)) return false;

            }
        }
        if (seedType.cutTreePrefab.HasPrefab)
        {
            seedType.cutTreePrefab.Instantiate(transform.position, transform.rotation); 
        }

        if (seedType.cutTile)
        {
            var tilemap = gridSystem.GetComponentInChildrenWithHypertag<Tilemap>(seedType.cutTileLayer);
            if (tilemap)
            {
                tilemap.SetTile(position.xy0(), seedType.cutTile);
            }
        }

        Destroy(gameObject);

        return true;
    }

    private bool FindEmptyCriteria(GridObject obj, Vector2Int pos)
    {
        if (obj != null) return false;

        // Check tiles on this position
        var tiles = gridSystem.GetTiles(pos);

        return (tiles.Count == 0);
    }
}
