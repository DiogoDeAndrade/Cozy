using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Life : MonoBehaviour
{
    [SerializeField] 
    public float            radius = 128;
    [SerializeField]
    private bool            enableLight = true;
    [SerializeField] 
    private bool            enableLightColorAnimation;
    [SerializeField, ShowIf(nameof(enableLightColorAnimation))]
    private float           lightAnimationDuration = 1.0f;
    [SerializeField, ShowIf(nameof(enableLightColorAnimation))]
    private Gradient        lightAnimationColor;
    [SerializeField]
    private ResourceHandler lifeLightHandler;

    SpriteRenderer  spriteRenderer;
    Light2D         lifeLight;
    float           lightColorAnimTimer;
    Lightfield      lightfield;
    Vector3         prevPos;

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

        if (lifeLightHandler)
        {
            lifeLightHandler.onResourceEmpty += ToggleLight;
            lifeLightHandler.onResourceNotEmpty += ToggleLight;
        }

        lightfield = GetComponentInParent<Lightfield>();
        lightfield.SetDirty();

        prevPos = transform.position;
    }

    private void ToggleLight(GameObject changeSource)
    {
        if (lifeLightHandler.normalizedResource > 0.0f)
        {
            lifeLight.FadeTo(1.0f, 0.5f);
        }
        else
        {
            lifeLight.FadeOut(0.5f);
        }

        lightfield?.SetDirty();
    }

    [Button("Init")]
    void Init()
    {
        if (enableLight)
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
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
            lifeLight.pointLightInnerRadius = 0.0f;
            lifeLight.pointLightOuterRadius = radius;
            lifeLight.shadowIntensity = 1.0f;
            lifeLight.falloffIntensity = 0.0f;
            lifeLight.SetSortingLayerToEverything();
        }
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

        if (prevPos != transform.position)
        {
            lightfield?.SetDirty();
            prevPos = transform.position;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    private void OnDestroy()
    {
        lightfield?.SetDirty();
    }
}
