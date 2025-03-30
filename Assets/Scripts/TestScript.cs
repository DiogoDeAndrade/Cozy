using UnityEngine;

public class TestScript : MonoBehaviour
{
    [SerializeField]
    private ParamPrefab<ItemPickup> testPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        testPrefab.Instantiate(transform.position, transform.rotation);        
    }
}
