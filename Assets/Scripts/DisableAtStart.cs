using UnityEngine;

public class DisableAtStart : MonoBehaviour
{
    void Awake()
    {
        gameObject.SetActive(false);
    }
}
