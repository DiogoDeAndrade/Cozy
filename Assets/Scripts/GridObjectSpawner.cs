using UnityEngine;
using System.Collections.Generic;

public class GridObjectSpawner : MonoBehaviour, ITurnExecute
{
    [SerializeField] private int                spawnCount = 1;
    [SerializeField] private int                spawnDelay = 5;
    [SerializeField] private int                spawnRadius = 1;
    [SerializeField] private List<GridObject>   prefabs;

    SpriteRenderer      spriteRenderer;
    List<GridObject>    spawnedObjects = new();
    int                 spawnTimer;
    GridSystem          gridSystem;

    void Start()
    {
        // Hide gizmo
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;

        gridSystem = GetComponentInParent<GridSystem>();
    }

    public void ExecuteTurn()
    {
        spawnedObjects.RemoveAll((obj) => obj == null);
        if ((spawnedObjects.Count < spawnCount) && (spawnTimer == 0))
        {
            spawnTimer = spawnDelay;
        }

        if (spawnTimer > 0)
        {
            spawnTimer--;
            if (spawnTimer <= 0)
            {
                if (gridSystem.FindVonNeumann(spawnRadius, transform.position, (obj) => obj == null, out var obj, out var pos))
                {
                    var newObject = Instantiate(prefabs.Random(), transform.parent);
                    newObject.transform.position = gridSystem.GridToWorld(pos);
                    spawnedObjects.Add(newObject);
                    if (spawnedObjects.Count < spawnCount) spawnTimer = spawnDelay;
                }
                else
                {
                    spawnTimer = spawnDelay;
                }
            }
        }
    }


}
