using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CultivationSystem : GridActionContainer
{
    Tilemap tilemap;

    protected override void Start()
    {
        base.Start();

        tilemap = GetComponent<Tilemap>();
    }

    public override void ActualGatherActions(GridObject subject, Vector2Int position, List<NamedAction> retActions)
    {
        // Check inventory of subject
        Inventory inventory = subject.GetComponent<Inventory>();
        if (inventory == null) return;

        foreach (var item in inventory)
        {
            if (item.item is ItemSeed seed)
            {
                // Check if this can be planeted on the current position
                var tile = tilemap.GetTile(position.xy0());
                if (seed.allowedSoil.Contains(tile))
                {
                    // This can be planted here
                    retActions.Add(new NamedAction
                    {
                        name = $"{verb} {item.item.displayName}",
                        action = (s, p) => RunAction(s, p, seed),
                        container = this,
                        combatTextEnable = true,
                        combatText = $"{combatText} {item.item.displayName}",
                        combatTextColor = item.item.displayTextColor,
                    });
                }
            }
        }
    }

    private bool RunAction(GridObject subject, Vector2Int position, ItemSeed seed)
    {
        Inventory inventory = subject.GetComponent<Inventory>();
        if (inventory == null) return false;

        // Clear the soil
        tilemap.SetTile(position.xy0(), null);

        // Instance the prefab
        if (seed.seedlingPrefab.HasPrefab)
        {
            seed.seedlingPrefab.Instantiate(gridSystem.GridToWorld(position), Quaternion.identity);
        }
        else if (seed.plantPrefab.HasPrefab)
        {
            seed.plantPrefab.Instantiate(gridSystem.GridToWorld(position), Quaternion.identity);
        }

        // Remove the seed from the inventory
        inventory.Remove(seed);

        return true;
    }
}
