
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Serializable]
    class ItemResource
    {
        public Item             item;
        public ResourceType     resType;
        public ResourceHandler  handler;
    }

    [SerializeField]
    private float               actionDelayTime = 0.5f;
    [SerializeField]
    private float               keyCacheDuration = 0.25f;
    [SerializeField] 
    private List<ItemResource>  itemResource;
    [SerializeField] 
    private Hypertag            tagPlayerSpawnPoint;
    [SerializeField] 
    private ResourceHandler     lightLifeHandler;
    [SerializeField] 
    private PlayerInput         playerInput;
    [SerializeField, InputPlayer(nameof(playerInput))]
    private InputControl        movementInput;
    [SerializeField, InputButton, InputPlayer(nameof(playerInput))] 
    private InputControl        continueDialogueControl;
    [SerializeField]
    private ParticleSystem      darkFadeOutPS;
    [SerializeField]
    private AudioClip           fadeOutSnd;
    [SerializeField]
    private ParticleSystem      darkFadeInPS;
    [SerializeField]
    private AudioClip           fadeInSnd;
    [SerializeField]
    private AudioClip           selectSnd;

    struct Action
    {
        public GridAction   action;
        public KeyCode      keyCode;
    }

    private GridSystem          gridSystem;
    private GridObject          gridObject;
    private MovementGridXY      movementGrid;
    private Inventory           inventory;
    private List<Action>        availableActions;
    private bool                actionsEnabled = true;
    private Stack<bool>         actionEnableStack = new();
    private Transform           origin;
    private SpriteRenderer      spriteRenderer;

    struct KeyCacheElem
    {
        public float   clickTime;
        public KeyCode keyCode;
    }

    private List<KeyCacheElem>       keyCache = new(); 
   
    void Start()
    {
        gridSystem = GetComponentInParent<GridSystem>();
        gridObject = GetComponent<GridObject>();
        movementGrid = GetComponent<MovementGridXY>();
        inventory = GetComponent<Inventory>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        var inventoryDisplay = FindFirstObjectByType<InventoryDisplay>();
        if (inventoryDisplay != null)
        {
            inventoryDisplay.SetInventory(inventory);
        }

        foreach (var r in itemResource)
        {
            r.handler = this.FindResourceHandler(r.resType);
            r.handler.enabled = inventory.HasItem(r.item);
        }

        origin = Hypertag.FindFirstObjectWithHypertag<Transform>(tagPlayerSpawnPoint);
        transform.position = gridSystem.Snap(origin.transform.position);

        inventory.onChange += OnInventoryUpdate;
        lightLifeHandler.onResourceEmpty += TeleportToOriginWithDelay;
        gridObject.onMoveEnd += MoveEnd;

        DialogueManager.Instance.onDialogueStart += onDialogueStart;
        DialogueManager.Instance.onDialogueEnd += onDialogueEnd;

        continueDialogueControl.playerInput = playerInput;
        movementInput.playerInput = playerInput;
    }

    void TeleportToOriginWithDelay(GameObject changeSource)
    {
        StartCoroutine(TeleportToOriginWithDelayCR(changeSource));
    }

    IEnumerator TeleportToOriginWithDelayCR(GameObject changeSource)
    {
        PushEnableAction(false);
        darkFadeOutPS.Play();
        if (fadeOutSnd) SoundManager.PlaySound(SoundType.PrimaryFX, fadeOutSnd);
        yield return new WaitForSeconds(0.25f);
        spriteRenderer.enabled = false;

        yield return new WaitForSeconds(0.75f);

        TeleportToOrigin(changeSource);

        darkFadeInPS.Play();
        if (fadeInSnd) SoundManager.PlaySound(SoundType.PrimaryFX, fadeInSnd);
        yield return new WaitForSeconds(0.25f);

        spriteRenderer.enabled = true;

        yield return new WaitForSeconds(0.5f);

        PopEnableAction();
    }

    private void TeleportToOrigin(GameObject changeSource)
    {
        // Player ran out of life light
        origin = Hypertag.FindFirstObjectWithHypertag<Transform>(tagPlayerSpawnPoint);
        transform.position = gridSystem.Snap(origin.transform.position);
        lightLifeHandler.ResetResource(true);
    }

    private void MoveEnd(Vector2Int sourcePos, Vector2Int destPos)
    {
        StartCoroutine(RunActionsDelayCR(0.0f));
    }

    private void OnInventoryUpdate(bool add, Item item, int slot)
    {
        foreach (var r in itemResource)
        {
            if (r.item == item)
            {
                r.handler.enabled = add;
            }
        }
    }

    void Update()
    {
        HandleOptions();

        keyCache.RemoveAll((k) => (Time.time - k.clickTime) > keyCacheDuration);

        if (actionsEnabled)
        {
            foreach (var action in availableActions)
            {
                if ((Input.GetKeyDown(action.keyCode)) || (IsKeyDownCached(action.keyCode)))
                {
                    if (action.action.RunAction(gridObject, gridObject.GetPositionFacing()))
                    {
                        if ((!action.action.hasSound) && (selectSnd)) SoundManager.PlaySound(SoundType.PrimaryFX, selectSnd, 1.0f, UnityEngine.Random.Range(0.75f, 1.25f));
                        if (action.action.ShouldRunTurn())
                        {
                            StartCoroutine(RunActionsDelayCR(actionDelayTime));
                        }
                    }
                }
                if (!actionsEnabled) break;
            }
        }
        else
        {
            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
                {
                    keyCache.Add(new KeyCacheElem
                    {
                        keyCode = key,
                        clickTime = Time.time,
                    });
                }
            }
        }

        if (DialogueManager.isTalking)
        {
            var moveVector = movementInput.GetAxis2();
            DialogueManager.SetInput(moveVector);
            
            if (continueDialogueControl.IsDown())
            {
                DialogueManager.Continue();
            }
        }
    }

    private bool IsKeyDownCached(KeyCode keyCode)
    {
        foreach (var k in keyCache)
        {
            if (k.keyCode == keyCode)
            {
                return ((Time.time - k.clickTime) < keyCacheDuration);
            }
        }

        return false;
    }

    IEnumerator RunActionsDelayCR(float delayTime)
    {
        PushEnableAction(false);

        if (delayTime > 0)
            yield return new WaitForSeconds(delayTime);

        try
        {
            ITurnExecute.ExecuteAllTurns();
        }
        catch(Exception e)
        {            
            Debug.LogError($"There was an exception running turns: {e.Message}");
            throw e;
        }

        PopEnableAction();
    }

    void HandleOptions()
    {
        var position = gridObject.GetPositionFacing();

        var actions = gridSystem.GetActions(gridObject, position);

        int cIndex = -1;

        availableActions = new();

        bool[] keys = new bool[26];
        foreach (var action in actions)
        {
            // Assign a key
            var verb = action.verb.ToUpper();
            char c = '\0';
            for (int i = 0; i < verb.Length; i++)
            {
                char cc = verb[i];
                cIndex = ((int)cc) - 'A';
                if (!keys[cIndex])
                {
                    c = cc;
                    break;
                }
            }
            if (c == '\0')
            {
                // Select the first unused letter
                for (int i = 0; i < keys.Length; i++)
                {
                    if (!keys[i])
                    {
                        c = (char)(i + 'A');
                        break;
                    }
                }
            }
            cIndex = ((int)c) - 'A';
            keys[cIndex] = true;
            availableActions.Add(new Action()
            {
                keyCode = (KeyCode)(KeyCode.A + cIndex),
                action = action
            });
        }
    }

    public List<string> GetAvailableOptions()
    {
        List<string> ret = new();
        if (availableActions != null)
        {
            foreach (var action in availableActions)
            {
                ret.Add($"({action.keyCode}): {action.action.verb}");
            }
        }

        return ret;
    }

    private void OnDrawGizmosSelected()
    {
        if (gridObject == null) return;

        var position = gridObject.GetPositionFacing();
        Gizmos.color = new Color(0.5f, 0.1f, 0.5f, 0.5f);
        Gizmos.DrawSphere(gridSystem.GridToWorld(position), 5.0f);
    }

    public int GetFacing() => gridObject.GetFacingDirection();

    public void PushEnableAction(bool b)
    {
        actionEnableStack.Push(actionsEnabled);
        
        actionsEnabled = b;
        movementGrid.enabled = b;
    }

    public void PopEnableAction()
    {
        actionsEnabled = actionEnableStack.Pop();

        movementGrid.enabled = actionsEnabled;
    }

    private void onDialogueStart(string dialogueKey)
    {
        PushEnableAction(false);
    }

    private void onDialogueEnd()
    {
        PopEnableAction();
    }
}
