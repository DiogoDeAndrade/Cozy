using System.Collections.Generic;
using UnityEngine;

public class PlayerResourceDisplay : MonoBehaviour
{
    [SerializeField] private ResourceBar resourceBarPrefab;

    struct Elem
    {
        public ResourceHandler  handler;
        public ResourceBar      bar;
        public CanvasGroup      cg;
    }

    List<Elem> activeResourceBars;

    Player player;

    void Start()
    {
    }

    void Update()
    {
        if (player == null)
        {
            player = FindAnyObjectByType<Player>();
            if (player != null)
            {
                var handlers = player.GetComponents<ResourceHandler>();
                activeResourceBars = new();
                foreach (var handler in handlers)
                {
                    var newBar = Instantiate(resourceBarPrefab, transform);
                    newBar.SetTarget(handler);

                    var cg = newBar.GetComponent<CanvasGroup>();
                    if (cg)
                    {
                        cg.alpha = 0.0f;
                        cg.FadeIn(0.25f);
                    }

                    activeResourceBars.Add(new Elem
                    {
                        bar = newBar,
                        handler = handler,
                        cg = cg,                        
                    });
                }
            }
        }

        if (player == null)
        {
            if (activeResourceBars != null)
            {
                foreach (var arb in activeResourceBars)
                {
                    Destroy(arb.bar.gameObject);
                }
                activeResourceBars = null;
            }
        }
        else
        {
            // Check if resource should be enabled
            if (activeResourceBars != null)
            {
                foreach (var arb in activeResourceBars)
                {
                    if (arb.cg)
                    {
                        if (arb.handler.enabled) arb.cg.FadeIn(0.25f);
                        else arb.cg.FadeOut(0.25f);
                    }
                    else
                    {
                        arb.bar.gameObject.SetActive(arb.handler.enabled);
                    }
                }
            }
        }
    }
}
