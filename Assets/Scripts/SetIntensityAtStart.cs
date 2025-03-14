using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SetIntensityAtStart : MonoBehaviour
{
    public float startIntensity;

    void Awake()
    {
        var light = GetComponent<Light2D>();
        light.intensity = startIntensity;
    }
}
