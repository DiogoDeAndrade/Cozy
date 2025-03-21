using System;
using UnityEngine;

public class Seedling : MonoBehaviour
{
    [SerializeField] private Item               seedType;
    [SerializeField] private ResourceHandler    lightHandler;
    [SerializeField] private ResourceHandler    waterHandler;
    [SerializeField] private ItemPickup         itemPickupPrefab;
    [SerializeField] private GameObject         plantPrefab;

    void Start()
    {
        waterHandler.SetResource(0.0f);
        lightHandler.onResourceEmpty += OnSeedlingDie;
        waterHandler.onChange += OnSeedlingWatered;
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

    private void OnSeedlingWatered(ResourceHandler.ChangeType changeType, float deltaValue, Vector3 changeSrcPosition, Vector3 changeSrcDirection, GameObject changeSource)
    {
        if (waterHandler.normalizedResource >= 1.0f)
        {
            Instantiate(plantPrefab, transform.position, transform.rotation);

            var context = InterfaceHelpers.GetFirstInterfaceComponent<UCExpression.IContext>();
            context?.SetVariable("PlantGrown", true);

            Destroy(gameObject);
        }
    }
}
