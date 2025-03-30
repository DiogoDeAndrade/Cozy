using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LifeLightUpdate : MonoBehaviour, ITurnExecute
{
    [SerializeField] private float      lifeLightGainPerTurn = 25;
    [SerializeField] private float      lifeLightLostPerTurn = 25;
    [SerializeField] private float      lightLowerBounds = 0.05f;

    [SerializeField] ResourceType       lifeLightType;

    Life            lifeObject;
    ResourceHandler lifeLightHandler;
    Lightfield      lightfield;

    public float GetLightLowerBound() => lightLowerBounds;

    void Start()
    {
        lifeLightHandler = this.FindResourceHandler(lifeLightType);
        lightfield = GetComponentInParent<Lightfield>();
    }

    public void ExecuteTurn()
    {
        float lightValue = lightfield.GetLight(transform.position);

        if (lightValue > lightLowerBounds)
        {
            lifeLightHandler.Change(ResourceHandler.ChangeType.Burst, lifeLightGainPerTurn, Vector3.zero, Vector3.zero, gameObject);
        }
        else
        {
            lifeLightHandler.Change(ResourceHandler.ChangeType.Burst, -lifeLightLostPerTurn, Vector3.zero, Vector3.zero, gameObject);
        }
    }
}
