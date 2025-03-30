using UnityEngine;
using System.Collections;

public class Darkfiend : MonoBehaviour, ITurnExecute
{
    [SerializeField] private ResourceType   darkResourceType;
    [SerializeField] private ParticleSystem normalPS;
    [SerializeField] private ParticleSystem deathPS;
    [SerializeField] private AudioClip      hurtSnd;
    [SerializeField] private AudioClip      deathSnd;
    [SerializeField] private float          attackSpeed = 200.0f;
    [SerializeField] private ResourceType   attackDamageType;
    [SerializeField] private float          attackDamage = 50.0f;
    [SerializeField] private AudioClip      attackSnd;

    ResourceHandler darkResourceHandler;
    SpriteRenderer  spriteRenderer;
    GridObject      gridObject;
    GridSystem      gridSystem;

    void Start()
    {
        darkResourceHandler = this.FindResourceHandler(darkResourceType);
        darkResourceHandler.onResourceEmpty += DarkResourceHandler_onResourceEmpty;
        darkResourceHandler.onChange += DarkResourceHandler_onChange;

        spriteRenderer = GetComponent<SpriteRenderer>();

        gridSystem = GetComponentInParent<GridSystem>();
        gridObject = GetComponent<GridObject>();
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
        normalPS.SetEmission(false);
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
        // Check if there's a player in neighborhood
        var player = gridSystem.FindVonNeumann(1, transform.position, (gridObject) => gridObject.GetComponent<Player>() != null);
        if (player)
        {
            // Try to move, won't be able to (by design)
            gridObject.MoveToGrid(gridObject.WorldToGrid(player.transform.position), new Vector2(attackSpeed, attackSpeed));
            if (attackSnd) SoundManager.PlaySound(SoundType.PrimaryFX, attackSnd);

            StartCoroutine(WaitMoveFinishAttackCR(player));
        }
    }

    IEnumerator WaitMoveFinishAttackCR(GridObject target)
    {
        while (gridObject.isMoving)
        {
            yield return null;
        }

        var handler = target.FindResourceHandler(attackDamageType);
        if (handler)
        {
            handler.Change(ResourceHandler.ChangeType.Burst, -attackDamage, transform.position, transform.position - target.transform.position, gameObject);
        }
    }
}
