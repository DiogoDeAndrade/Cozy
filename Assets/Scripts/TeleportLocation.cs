using UnityEngine;

public class TeleportLocation : HypertaggedObject
{
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        var cellSize = FindFirstObjectByType<Grid>()?.cellSize ?? Vector3.zero;

        Gizmos.color = Color.yellow;
        DebugHelpers.DrawBox(new Bounds(transform.position, cellSize));

        string n = name;
        DebugHelpers.DrawTextAt(new Vector3(transform.position.x, transform.position.y - cellSize.y * 0.5f, transform.position.z), Vector3.zero, 12, Color.yellow, n, false, true);
    }
#endif
}
