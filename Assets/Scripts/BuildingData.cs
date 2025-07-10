using UnityEngine;

[CreateAssetMenu(menuName = "FarmLogistics/BuildingData")]
public class BuildingData : ScriptableObject
{
    public string displayName;
    public Sprite icon;
    public GameObject prefab;
    public int buildCost;

    // Production logic
    public ProductData outputProduct;         // What this building produces
    public int outputAmount = 1;              // How much per cycle
    public float productionTime = 5f;         // Seconds per cycle

    public int inputStorageLimit;

    public int outputStorageLimit;

    public InputProduct[] inputProducts;       // What is required (can be empty)

    public Vector2Int size = Vector2Int.one; // Default 1x1
    public BuildingRole buildingRole;
} 

public enum BuildingRole
{
    Producer,    // Farms (no input)
    Factory,     // Factory (Require Input)
    Store,       // Customer-facing stores
}

[System.Serializable]
public struct InputProduct{

    public ProductData productData;
    public int amount;

}