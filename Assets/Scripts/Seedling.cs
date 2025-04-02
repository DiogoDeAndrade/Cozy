using Mono.Cecil;
using System.Collections.Generic;
using UnityEngine;

public class Seedling : GridActionContainer
{
    [SerializeField, PrefabParam] 
    private ItemSeed       seedType;
    [SerializeField] 
    private ResourceType       lightResource;
    [SerializeField] 
    private ResourceType       waterResource;
    [SerializeField]
    private float              quantityPerWatering;
    [SerializeField, PrefabParam]
    private float              quantityMultiplier = 1.0f;
    [SerializeField] 
    private ItemPickup         itemPickupPrefab;

    private ResourceHandler lightHandler;
    private ResourceHandler waterHandler;

    protected override void Start()
    {
        base.Start();

        lightHandler = this.FindResourceHandler(lightResource);
        lightHandler.onResourceEmpty += OnSeedlingDie;
        waterHandler = this.FindResourceHandler(waterResource);
        waterHandler.SetResource(0.0f);
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
            seedType.plantPrefab.Instantiate(transform.position, transform.rotation);

            var context = InterfaceHelpers.GetFirstInterfaceComponent<UCExpression.IContext>();
            context?.SetVariable("PlantGrown", true);

            Destroy(gameObject);
        }
    }

    public override void ActualGatherActions(GridObject subject, Vector2Int position, List<NamedAction> retActions)
    {
        var thisHandler = this.FindResourceHandler(waterResource);
        if (thisHandler == null) return;
        if (thisHandler.normalizedResource >= 1.0f) return;

        var thatHandler = subject.FindResourceHandler(waterResource);
        if (thatHandler == null) return;
        if (thatHandler.resource < quantityPerWatering) return;

        retActions.Add(new NamedAction
        {
            name = verb,
            action = RunAction,
            container = this
        });
    }

    protected bool RunAction(GridObject subject, Vector2Int position)
    {
        var thisHandler = this.FindResourceHandler(waterResource);
        var thatHandler = subject.FindResourceHandler(waterResource);

        thisHandler.Change(ResourceHandler.ChangeType.Burst, quantityPerWatering * quantityMultiplier, subject.transform.position, Vector3.zero, subject.gameObject, true);
        thatHandler.Change(ResourceHandler.ChangeType.Burst, -quantityPerWatering, transform.position, Vector3.zero, gameObject, true);

        return true;
    }
}
