using UnityEngine;

public class LifeLightUpdate : MonoBehaviour, ITurnExecute
{
    [SerializeField] private float lifeLightLostPerTurn = 25;
    [SerializeField] private float lifeLightGainPerTurn = 25;

    [SerializeField] ResourceHandler lifeLightHandler;

    void Start()
    {
    }

    public void ExecuteTurn()
    {
        lifeLightHandler.Change(ResourceHandler.ChangeType.Burst, -lifeLightLostPerTurn, Vector3.zero, Vector3.zero, gameObject);
    }
}
