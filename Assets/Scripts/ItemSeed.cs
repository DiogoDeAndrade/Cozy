using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Tree Seed", menuName = "Cozy Lights/Data/Item")]
public class ItemSeed : Item
{
    [Header("Plant Properties")]
    public Sprite                   plantSprite;
    public Color                    plantSpriteColor = Color.white;
    public TileBase[]               allowedSoil;
    public ParamPrefab<Seedling>    seedlingPrefab;
    public ParamPrefab<Plant>       plantPrefab;
    public ParamPrefab<GameObject>  cutTreePrefab;
    public TileBase                 cutTile;
    [ShowIf(nameof(hasCutTile))]
    public Hypertag                 cutTileLayer;

    bool hasCutTile => cutTile != null;
}
