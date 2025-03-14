using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LifeLightUpdate : MonoBehaviour, ITurnExecute
{
    [SerializeField] private float      lifeLightGainPerTurn = 25;
    [SerializeField] private float      lifeLightLostPerTurn = 25;

    [SerializeField] ResourceHandler    lifeLightHandler;
    [SerializeField] LayerMask          obstacleLayers;
    
    [SerializeField] bool               debugRaycasts = false;

    Vector2 size;
    Life    lifeObject;

    void Start()
    {
        size = GetComponentInParent<GridSystem>().cellSize * 0.5f;
        lifeObject = GetComponent<Life>();
    }

    public void ExecuteTurn()
    {
        if (LifeLightUtils.IsLit5(transform.position, size, lifeLightHandler.type, obstacleLayers, lifeObject))
        {
            lifeLightHandler.Change(ResourceHandler.ChangeType.Burst, lifeLightGainPerTurn, Vector3.zero, Vector3.zero, gameObject);
        }
        else
        {
            lifeLightHandler.Change(ResourceHandler.ChangeType.Burst, -lifeLightLostPerTurn, Vector3.zero, Vector3.zero, gameObject);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (!debugRaycasts) return;

        // Find all life
        float hitDistance = 0.0f;

        var lifeObjects = FindObjectsByType<Life>(FindObjectsSortMode.None);

        foreach (var lifeObject in lifeObjects)
        {
            Vector3 targetPos = lifeObject.transform.position;

            bool hasLOS = LifeLightUtils.IsLit5(lifeObject, transform.position, size, lifeLightHandler.type, obstacleLayers, ref hitDistance);

            if (hasLOS)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, targetPos);
            }
            else
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, transform.position + (targetPos - transform.position).SafeNormalized() * hitDistance);
                Handles.DrawDottedLine(transform.position, targetPos, 5);
            }
        }
    }
#endif
}
