using NaughtyAttributes;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ProximitySound : MonoBehaviour
{
    [SerializeField] private Hypertag[] tags;
    [SerializeField] private float      radius = 50.0f;
    [SerializeField] private float      fadeTime = 0.5f;
    [SerializeField] private AudioClip  audioClip;
    [SerializeField, MinMaxSlider(0.0f, 1.0f)] 
    private Vector2 volume = Vector2.one;
    [SerializeField, MinMaxSlider(0.1f, 2.0f)] 
    private Vector2 pitch = Vector2.one;
    [SerializeField, ShowIf(nameof(hasTilemap))]
    private TileBase[] tiles;

    AudioSource audioSource;
    Tilemap     tilemap;

    bool hasTilemap => (GetComponent<Tilemap>() != null);

    void Start()
    {
        tilemap = GetComponent<Tilemap>();
    }

    void Update()
    {
        bool soundPlay = false;

        if (tilemap)
        {
            foreach (var obj in HypertaggedObject.Get<GridObject>(tags))
            {
                // Check if the neighborhood of this object has the given tiles
                var tilePos = tilemap.WorldToCell(obj.transform.position);
                int tileRadiusX = Mathf.CeilToInt(radius / tilemap.cellSize.x);
                int tileRadiusY = Mathf.CeilToInt(radius / tilemap.cellSize.y);

                for (int y = tilePos.y - tileRadiusY; y <= tilePos.y + tileRadiusY; y++)
                {
                    for (int x = tilePos.x - tileRadiusY; x <= tilePos.x + tileRadiusX; x++)
                    {
                        var tile = tilemap.GetTile(new Vector3Int(x, y, 0));
                        if (tiles.Contains(tile))
                        {
                            soundPlay = true;
                            break;
                        }
                    }

                    if (soundPlay) break;
                }

            }
        }
        else
        {
            var objects = HypertaggedObject.GetInRadius<GridObject>(tags, transform.position, radius).ToList();
            soundPlay = objects.Count > 0;
        }

        if (soundPlay)
        {
            if (audioSource == null)
            {
                audioSource = SoundManager.PlayLoopSound(SoundType.Background, audioClip, volume.Random(), pitch.Random());
                if (audioSource)
                {
                    if (fadeTime > 0.0f)
                    {
                        audioSource.volume = 0.0f;
                        audioSource.FadeTo(1.0f, fadeTime);
                    }
                }
            }
        }
        else
        {
            if (audioSource)
            {
                var tmp = audioSource;
                audioSource.FadeTo(0.0f, fadeTime).Done(() =>
                {
                    tmp.Stop();
                });
                audioSource = null;
            }
        }
    }

    private void OnDestroy()
    {
        if (audioSource)
        {
            audioSource.Stop();
            audioSource = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.0f, 0.5f, 0.0f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
