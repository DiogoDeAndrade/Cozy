using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Tilemaps;
using System;

public class Lightfield : MonoBehaviour, ITurnExecute
{
    [SerializeField] private LayerMask  lightObstructionMask;
    [SerializeField] private bool       debugView;
    
    private bool        dirty;
    private Vector2Int  bufferSize;
    private Vector3Int  lightfieldOrigin;
    private Vector2     limits;
    private float[,]    lightfield;
    private Grid        grid;

    void Start()
    {      
    }

    public void SetDirty()
    {
        dirty = true;
        grid = GetComponent<Grid>();
    }

    public int GetExecutionOrder() => -int.MaxValue;

    public void ExecuteTurn()
    {
        if (dirty)
        {
            UpdateLightfield();
        }
    }

    [Button("Update Now")]
    void UpdateLightfield()
    {
        ComputeBufferSize();

        if ((lightfield == null) || (lightfield.GetLength(0) != bufferSize.x) || (lightfield.GetLength(1) != bufferSize.y))
        {
            lightfield = new float[bufferSize.x, bufferSize.y];
        }
        else
        {
            for (int y = 0; y < bufferSize.y; y++)
            {
                for (int x = 0; x < bufferSize.x; x++)
                {
                    lightfield[x, y] = 0f;
                }
            }
        }

        limits = new Vector2(float.MaxValue, -float.MaxValue);

        ComputeLightfieldFromLives();

        dirty = false;
    }

    void ComputeLightfieldFromLives()
    {
        if (grid == null)
        {
            grid = GetComponent<Grid>();
        }

        var lives = FindObjectsByType<Life>(FindObjectsSortMode.None);
        foreach (var life in lives)
        {
            if (!life.enabled || life.radius <= 0f) continue;

            Vector3 lightWorldPos = life.transform.position;
            Vector3Int lightGridPos = grid.WorldToCell(lightWorldPos);

            float maxDist = life.radius;

            int radiusInCells = Mathf.CeilToInt(maxDist / grid.cellSize.x);

            for (int dy = -radiusInCells; dy <= radiusInCells; dy++)
            {
                for (int dx = -radiusInCells; dx <= radiusInCells; dx++)
                {
                    Vector3Int cell = lightGridPos + new Vector3Int(dx, dy, 0);
                    Vector3 worldPos = grid.GetCellCenterWorld(cell);
                    float dist = Vector3.Distance(worldPos, lightWorldPos);

                    if (dist > maxDist)
                        continue;

                    // Check LOS
                    Vector3[] offsets = new Vector3[]
                    {
                    Vector3.zero, // center
                    new Vector3(-0.5f, -0.5f), // bottom-left
                    new Vector3(0.5f, -0.5f),  // bottom-right
                    new Vector3(-0.5f, 0.5f),  // top-left
                    new Vector3(0.5f, 0.5f)    // top-right
                    };

                    int hits = 0;
                    foreach (var offset in offsets)
                    {
                        Vector3 from = worldPos + Vector3.Scale(offset, grid.cellSize);
                        if (!Physics2D.Linecast(from, lightWorldPos, lightObstructionMask))
                        {
                            hits++;
                        }
                    }

                    float losFactor = hits / 5.0f;
                    float intensity = Mathf.Lerp(1f, 0f, dist / maxDist);
                    float total = intensity * losFactor;

                    Vector2Int local = new Vector2Int(cell.x - lightfieldOrigin.x, cell.y - lightfieldOrigin.y);
                    if (local.x >= 0 && local.y >= 0 && local.x < bufferSize.x && local.y < bufferSize.y)
                    {
                        lightfield[local.x, local.y] += total;
                        limits.x = Mathf.Min(limits.x, lightfield[local.x, local.y]);
                        limits.y = Mathf.Max(limits.y, lightfield[local.x, local.y]);
                    }
                }
            }
        }
    }


    void ComputeBufferSize()
    {
        var tilemaps = GetComponentsInChildren<Tilemap>();
        if (tilemaps.Length == 0)
        {
            Debug.LogWarning("No Tilemaps found.");
            return;
        }

        bool initialized = false;
        BoundsInt combinedBounds = new BoundsInt();

        foreach (var tilemap in tilemaps)
        {
            var bounds = tilemap.cellBounds;

            // Convert tilemap-local cell positions to world cell positions
            Vector3Int offset = Vector3Int.FloorToInt(tilemap.transform.localPosition);
            bounds.position += offset;

            if (!initialized)
            {
                combinedBounds = bounds;
                initialized = true;
            }
            else
            {
                combinedBounds = UnionBounds(combinedBounds, bounds);
            }
        }

        bufferSize = combinedBounds.size.xy();
        lightfieldOrigin = combinedBounds.min;
    }

    BoundsInt UnionBounds(BoundsInt a, BoundsInt b)
    {
        int xMin = Mathf.Min(a.xMin, b.xMin);
        int yMin = Mathf.Min(a.yMin, b.yMin);
        int xMax = Mathf.Max(a.xMax, b.xMax);
        int yMax = Mathf.Max(a.yMax, b.yMax);

        return new BoundsInt(xMin, yMin, 0, xMax - xMin, yMax - yMin, 1);
    }

    void OnDrawGizmos()
    {
        if ((!debugView) || (lightfield == null))
        {
            return;
        }

        Vector3 cellSize = grid.cellSize;

        for (int y = 0; y < bufferSize.y; y++)
        {
            for (int x = 0; x < bufferSize.x; x++)
            {
                float value = lightfield[x, y];
                float t = Mathf.InverseLerp(limits.x, limits.y, value);
                Color color = Color.Lerp(Color.black, Color.yellow, t);
                color.a = 0.25f;

                Vector3Int  cellPosition = new Vector3Int(x + lightfieldOrigin.x, y + lightfieldOrigin.y, 0);
                Vector3     worldPosition = grid.CellToWorld(cellPosition) + cellSize * 0.5f;

                Gizmos.color = color;
                Gizmos.DrawCube(worldPosition, cellSize * 0.95f); // Slightly smaller for spacing
            }
        }
    }

    public float GetLight(Vector3 position)
    {
        if (dirty)
        {
            UpdateLightfield();
        }

        if (lightfield == null)
            return 0f;

        Vector3Int cellPos = grid.WorldToCell(position);
        Vector2Int local = new Vector2Int(cellPos.x - lightfieldOrigin.x, cellPos.y - lightfieldOrigin.y);

        if (local.x < 0 || local.y < 0 || local.x >= bufferSize.x || local.y >= bufferSize.y)
            return 0f;

        return lightfield[local.x, local.y];
    }
}
