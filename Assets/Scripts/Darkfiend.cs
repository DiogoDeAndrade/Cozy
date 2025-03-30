using UnityEngine;
using System.Collections;

public class Darkfiend : MonoBehaviour, ITurnExecute
{
    [SerializeField] private ResourceType   darkResourceType;
    [SerializeField] private ParticleSystem normalPS;
    [SerializeField] private ParticleSystem deathPS;
    [SerializeField] private AudioClip      hurtSnd;
    [SerializeField] private AudioClip      deathSnd;

    ResourceHandler darkResourceHandler;
    SpriteRenderer  spriteRenderer;

    void Start()
    {
        darkResourceHandler = this.FindResourceHandler(darkResourceType);
        darkResourceHandler.onResourceEmpty += DarkResourceHandler_onResourceEmpty;
        darkResourceHandler.onChange += DarkResourceHandler_onChange;

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void DarkResourceHandler_onChange(ResourceHandler.ChangeType changeType, float deltaValue, Vector3 changeSrcPosition, Vector3 changeSrcDirection, GameObject changeSource)
    {
        if (deltaValue < 0)
        {
            if (hurtSnd) SoundManager.PlaySound(SoundType.PrimaryFX, hurtSnd, 1.0f, Random.Range(0.8f, 1.1f));
        }
    }

    private void DarkResourceHandler_onResourceEmpty(GameObject changeSource)
    {
        // Kill this enemy 
        StartCoroutine(KillEnemyCR(changeSource));
    }

    IEnumerator KillEnemyCR(GameObject changeSource)
    {
        deathPS.Play();
        normalPS.SetEmitter(false);
        if (deathSnd) SoundManager.PlaySound(SoundType.PrimaryFX, deathSnd);
        yield return new WaitForSeconds(0.25f);
        spriteRenderer.enabled = false;

        while ((deathPS.particleCount > 0) || (normalPS.particleCount > 0))
        {
            yield return null;
        }

        Destroy(gameObject);
    }


    public void ExecuteTurn()
    {
        
    }

}
