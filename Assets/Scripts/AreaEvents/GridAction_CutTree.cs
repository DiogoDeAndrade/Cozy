using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class GridAction_CutTree : GridAction
{
    [SerializeField, Header("Cut Tree")]
    private Item seedToSpawn;
    [SerializeField]
    private ItemPickup  itemPickupPrefab;
    [SerializeField]
    private GameObject objectToReplace;

    protected override void ActualGatherActions(GridObject subject, Vector2Int position, List<GridAction> retActions)
    {
        retActions.Add(this);
    }

    protected override bool ActualRunAction(GridObject subject, Vector2Int position)
    {
        if ((itemPickupPrefab) && (seedToSpawn))
        {
            var newItem = Instantiate(itemPickupPrefab, new Vector3(transform.position.x, transform.position.y - gridSystem.cellSize.y, transform.position.z), Quaternion.identity);
            newItem.SetItem(seedToSpawn);
        }
        if (objectToReplace)
        {
            Instantiate(objectToReplace, transform.position, transform.rotation); 
        }    

        Destroy(gameObject);

        return true;
    }
}
