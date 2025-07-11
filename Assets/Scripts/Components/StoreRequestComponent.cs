using Scripts;
using UnityEngine;

/// <summary>
/// Handles stock monitoring and transport requests for store buildings.
/// Monitors storeStock levels and creates requests when inventory is running low for restocking.
/// </summary>
public class StoreRequestComponent : BaseRequestComponent
{
    /// <summary>
    /// Store-specific defaults: Higher threshold since stores serve customers directly,
    /// and faster check interval for more responsive restocking.
    /// </summary>
    private void Awake()
    {
        // Store-specific default values
        stockThreshold = 0.5f; // Request restocking at 50% (vs 25% for factories)
        customCheckInterval = 3f; // Check every 3 seconds (vs 7 for factories)
    }

    /// <summary>
    /// Checks all products that this store sells and monitors their stock levels.
    /// </summary>
    protected override void CheckAllProducts()
    {
        foreach (var product in baseBuilding.buildingData.storeStock)
        {
            CheckProductStock(product.productData);
        }
    }

    /// <summary>
    /// Checks the stock level of a specific store product and creates a transport request if needed.
    /// </summary>
    /// <param name="productData">The store product to check stock for.</param>
    protected override void CheckProductStock(ProductData productData)
    {
        int currentStock = GetCurrentStock(productData);
        int maxCapacity = GetProductCapacity(productData);

        if (currentStock <= (maxCapacity * stockThreshold))
        {
            if (!deliveringProducts.ContainsKey(productData))
            {
                int amountToRequest = maxCapacity - currentStock;
                TransportRequest request = CreateRequest(productData, amountToRequest);
                deliveringProducts.Add(productData, request);
            }
        }
    }

    /// <summary>
    /// Gets the storage capacity for a specific store product.
    /// Retrieves the capacity directly from the store's product configuration.
    /// </summary>
    /// <param name="productData">The store product to get capacity for.</param>
    /// <returns>The storage capacity for the specified store product.</returns>
    protected override int GetProductCapacity(ProductData productData)
    {
        // Find the capacity for this specific product in the store's configuration
        foreach (var storeProduct in baseBuilding.buildingData.storeStock)
        {
            if (storeProduct.productData == productData)
            {
                return storeProduct.amount; // In storeStock, amount represents capacity
            }
        }
        
        Debug.LogWarning($"Store product {productData.name} not found in storeStock configuration!");
        return 0;
    }

    /// <summary>
    /// Gets the current stock level for a specific store product from the store's inventory.
    /// </summary>
    /// <param name="productData">The store product to get current stock for.</param>
    /// <returns>The current stock level for the specified store product.</returns>
    protected override int GetCurrentStock(ProductData productData)
    {
        Store store = baseBuilding as Store;
        if (store != null && store.storeStock.TryGetValue(productData, out int currentStock))
        {
            return currentStock;
        }
        
        return 0; // If not found, assume empty stock
    }
}