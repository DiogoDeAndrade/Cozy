using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class OptionsDisplay : MonoBehaviour
{
    private Player              player;
    private CanvasGroup         canvasGroup;
    private TextMeshProUGUI[]   options;
    private RectTransform       rectTransform;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        rectTransform = transform as RectTransform;

        options = GetComponentsInChildren<TextMeshProUGUI>(true);
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
            for (int i = availableOptions.Count; i < options.Length; i++)
            {
                options[i].gameObject.SetActive(false);
            }
        }
    }
}
