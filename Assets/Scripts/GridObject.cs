using UnityEngine;

public class GridObject : MonoBehaviour
{
    [SerializeField] private bool alwaysUpdate;

    Grid grid;
    private void Awake()
    {
        grid = GetComponentInParent<Grid>();
        if (grid == null)
        {
            grid = FindFirstObjectByType<Grid>();
        }

        ClampToGrid();
    }

    void ClampToGrid()
    { 
        // Set to grid
        var gridPos = grid.WorldToCell(transform.position);

        transform.position = grid.CellToWorld(gridPos) + new Vector3(8.0f, 8.0f, 0.0f);

        enabled = alwaysUpdate;
    }

    void Update()
    {
        ClampToGrid();
    }
}
