using NaughtyAttributes;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private Item item;

    public void SetItem(Item item)
    {
        this.item = item;
    }

    [Button("Update visuals")]
    private void Start()
    {
        if (item != null)
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = item.displaySprite;
            spriteRenderer.color = item.displaySpriteColor;

            var gridActionPickup = GetComponent<GridAction_Pickup>();
            gridActionPickup.SetItem(item);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
