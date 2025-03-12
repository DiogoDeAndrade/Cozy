using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class OptionsDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ignoreText;
    
    private Player                  player;
    private CanvasGroup             canvasGroup;
    private List<TextMeshProUGUI>   options;
    private RectTransform           rectTransform;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        rectTransform = transform as RectTransform;

        options = new();

        var childTexts = GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (var text in childTexts)
        {
            if (text != ignoreText) options.Add(text);
        }
    }

    private void Update()
    {
        if (player == null)
        {
            player = FindAnyObjectByType<Player>();
        }
        if (player == null)
        {
            canvasGroup.FadeOut(0.25f);
            return;
        }

        var availableOptions = player.GetAvailableOptions();
        if (availableOptions.Count == 0)
        {
            canvasGroup.FadeOut(0.25f);
        }
        else
        {
            canvasGroup.FadeIn(0.25f);

            for (int i = 0; i < availableOptions.Count; i++)
            {
                options[i].text = availableOptions[i];
                options[i].gameObject.SetActive(true);
            }
            for (int i = availableOptions.Count; i < options.Count; i++)
            {
                options[i].gameObject.SetActive(false);
            }
        }
    }
}
