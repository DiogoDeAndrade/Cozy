using System;
using UnityEngine;

public class Seedling : MonoBehaviour
{
    [SerializeField] private Item               seedType;
    [SerializeField] private ResourceHandler    lightHandler;
    [SerializeField] private ItemPickup         itemPickupPrefab;

    void Start()
    {
        lightHandler.onResourceEmpty += OnSeedlingDie;
    }

    private void OnSeedlingDie(GameObject changeSource)
    {
        // Create item pickup
        var newItemPickup = Instantiate(itemPickupPrefab, transform.position, transform.rotation);
        newItemPickup.SetItem(seedType);

        var context = InterfaceHelpers.GetFirstInterfaceComponent<UCExpression.IContext>();
        context?.SetVariable("SeedlingDied", true);

        Destroy(gameObject);
    }
}
