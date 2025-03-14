using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Life : MonoBehaviour
{
    [SerializeField] 
    public float   radius = 128;
    [SerializeField] 
    private bool    enableLightColorAnimation;
    [SerializeField, ShowIf(nameof(enableLightColorAnimation))]
    private float    lightAnimationDuration = 1.0f;
    [SerializeField, ShowIf(nameof(enableLightColorAnimation))]
    private Gradient lightAnimationColor;

    SpriteRenderer spriteRenderer;
    Light2D lifeLight;
    float lightColorAnimTimer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        if (radius > 0)
        {
            Init();
        }

        if (enableLightColorAnimation)
        {
            lightColorAnimTimer = lightAnimationDuration;
        }
    }

    [Button("Init")]
    void Init()
    { 
        // Life emits light
        lifeLight = GetComponentInChildren<Light2D>();
        if (lifeLight == null)
        {
            var lightObject = new GameObject("Light");
            lightObject.transform.parent = transform;
            lifeLight = lightObject.AddComponent<Light2D>();
        }
        lifeLight.transform.localPosition = Vector3.zero;
        lifeLight.transform.localRotation = Quaternion.identity;
        lifeLight.lightType = Light2D.LightType.Point;
        lifeLight.color = spriteRenderer.color;
        lifeLight.intensity = 1;
        lifeLight.pointLightInnerRadius = radius * 0.5f;
        lifeLight.pointLightOuterRadius = radius;
        lifeLight.shadowIntensity = 1.0f;
        lifeLight.falloffIntensity = 0.3f;
    }

    private void Update()
    {
        if (enableLightColorAnimation)
        {
            lightColorAnimTimer -= Time.deltaTime;
            if (lightColorAnimTimer < 0.0f)
            {
                lightColorAnimTimer = lightAnimationDuration;
            }
            lifeLight.color = lightAnimationColor.Evaluate(1.0f - lightColorAnimTimer / lightAnimationDuration);
            spriteRenderer.color = lifeLight.color;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
