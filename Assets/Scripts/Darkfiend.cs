using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Darkfiend : MonoBehaviour, ITurnExecute
{
    [SerializeField] private float          moveSpeed = 100.0f;
    [SerializeField] private ResourceType   darkResourceType;
    [SerializeField] private ParticleSystem spawnPS;
    [SerializeField] private ParticleSystem normalPS;
    [SerializeField] private ParticleSystem deathPS;
    [SerializeField] private AudioClip      hurtSnd;
    [SerializeField] private AudioClip      deathSnd;
    [SerializeField] private int            detectionRadius;
    [SerializeField] private LayerMask      maskLOS;
    [SerializeField] private float          attackSpeed = 200.0f;
    [SerializeField] private ResourceType   attackDamageType;
    [SerializeField] private float          attackDamage = 50.0f;
    [SerializeField] private AudioClip      attackSnd;

    ResourceHandler     darkResourceHandler;
    SpriteRenderer      spriteRenderer;
    GridObject          gridObject;
    GridSystem          gridSystem;
    Lightfield          lightfield;
    LifeLightUpdate     lightUpdate;
    List<Vector2Int>    currentPath;
    int                 currentPathIndex = 0;
    Vector3             pathTargetPos;
    GridObject          targetPlayer;
    int                 turnSinceLastMove = 0;
    Vector3             spawnPos;

    void Start()
    {
        darkResourceHandler = this.FindResourceHandler(darkResourceType);
        darkResourceHandler.onResourceEmpty += DarkResourceHandler_onResourceEmpty;
        darkResourceHandler.onChange += DarkResourceHandler_onChange;

        spriteRenderer = GetComponent<SpriteRenderer>();

        gridSystem = GetComponentInParent<GridSystem>();
        gridObject = GetComponent<GridObject>();
        lightfield = GetComponentInParent<Lightfield>();
        lightUpdate = GetComponent<LifeLightUpdate>();

        spawnPos = gridObject.Snap(transform.position);

        if (spawnPS)
        {
            StartCoroutine(SpawnCR());
        }
    }

    IEnumerator SpawnCR()
    {
        spriteRenderer.enabled = false;
        spawnPS.Play();
        yield return new WaitForSeconds(0.25f);
        spriteRenderer.enabled = true;
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
        turnSinceLastMove++;

        if (targetPlayer == null)
        {
            if (FollowPath()) return;
        }

        // Check if we're standing on light
        if (lightfield.GetLight(transform.position) > lightUpdate.GetLightLowerBound())
        {
            currentPath = lightfield.FindDarkPath(transform.position, (pos) => !gridSystem.CheckCollision(pos, gridObject), lightUpdate.GetLightLowerBound());
            currentPathIndex = 1; // Start in 1 because the initial position is included in the path
            targetPlayer = null;

            if (FollowPath()) return; 
        }

        if ((targetPlayer != null) && (currentPath != null))
        {
            // Check if we need to recompute the path
            if (pathTargetPos != targetPlayer.transform.position)
            {
                // Check if we have LOS
                if (!Physics2D.Linecast(transform.position, targetPlayer.transform.position))
                {
                    pathTargetPos = targetPlayer.transform.position;
                }
            }

            currentPath = lightfield.FindDarkPath(transform.position, pathTargetPos, (pos) => !gridSystem.CheckCollision(pos, gridObject), lightUpdate.GetLightLowerBound(), detectionRadius * 2);
            currentPathIndex = 1; // Start in 1 because the initial position is included in the path

            if (FollowPath())
            {
                return;
            }
        }

        // Check if there's a player in neighborhood for attack        
        if (gridSystem.FindVonNeumann(1, transform.position, IsPlayer, out var player, out var pos))
        {
            // Try to move, won't be able to (by design)
            gridObject.MoveToGrid(gridObject.WorldToGrid(player.transform.position), new Vector2(attackSpeed, attackSpeed));
            if (attackSnd) SoundManager.PlaySound(SoundType.PrimaryFX, attackSnd);

            StartCoroutine(WaitMoveFinishAttackCR(player));

            return;
        }

        // Get ready to chase the player        
        if (gridSystem.FindRadius(detectionRadius, transform.position, IsPlayerInLOS, out player, out pos))
        {
            targetPlayer = player;
            pathTargetPos = player.transform.position;
            currentPath = lightfield.FindDarkPath(transform.position, player.transform.position, (pos) => !gridSystem.CheckCollision(pos, gridObject), lightUpdate.GetLightLowerBound(), detectionRadius * 2);
            currentPathIndex = 1; // Start in 1 because the initial position is included in the path

            if (FollowPath()) return;
        }

        // Go back to the spawn position
        if ((turnSinceLastMove > 10) && (transform.position != spawnPos))
        {
            currentPath = lightfield.FindDarkPath(transform.position, spawnPos, (pos) => !gridSystem.CheckCollision(pos, gridObject), lightUpdate.GetLightLowerBound(), detectionRadius * 2);
            currentPathIndex = 1; // Start in 1 because the initial position is included in the path
            targetPlayer = null;

            if (FollowPath()) return;
        }
    }

    private bool IsPlayer(GridObject obj)
    {
        if (obj == null) return false;
        return obj.GetComponent<Player>() != null;
    }

    private bool IsPlayerInLOS(GridObject obj)
    {
        if (obj == null) return false;
        if (obj.GetComponent<Player>() == null) return false;

        if (Physics2D.Linecast(transform.position, obj.transform.position, maskLOS)) return false;

        return true;
    }

    bool FollowPath()
    {
        if (currentPath == null) return false;
        if (currentPath.Count <= currentPathIndex) return false;

        var nextPos = currentPath[currentPathIndex];
        if (gridObject.MoveToGrid(nextPos, new Vector2(moveSpeed, moveSpeed)))
        {
            turnSinceLastMove = 0;

            currentPathIndex++;
            if (currentPathIndex >= currentPath.Count)
            {
                currentPath = null;
            }
            return true;
        }
        return false;
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

    void OnDrawGizmosSelected()
    {
        var grid = GetComponentInParent<Grid>();
        if (grid == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius * grid.cellSize.x);

        if (currentPath == null) return;

        for (int i = 0; i < currentPath.Count - 1; i++)
        {
            var p1 =gridSystem.GridToWorld(currentPath[i]);
            var p2 = gridSystem.GridToWorld(currentPath[i + 1]);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(p1, p2);
        }
    }
}
