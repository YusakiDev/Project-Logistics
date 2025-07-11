using Scripts;
using UnityEngine;

/// <summary>
/// Handles stock monitoring and transport requests for production buildings (factories).
/// Monitors inputStock levels and creates requests when input materials are running low.
/// </summary>
public class BuildingRequestComponent : BaseRequestComponent
{
    /// <summary>
    /// Checks all input products that this building requires for production.
    /// </summary>
    protected override void CheckAllProducts()
    {
        foreach (var product in baseBuilding.buildingData.inputProducts)
        {
            CheckProductStock(product.productData);
        }
    }

    /// <summary>
    /// Checks the stock level of a specific input product and creates a transport request if needed.
    /// </summary>
    /// <param name="productData">The input product to check stock for.</param>
    protected override void CheckProductStock(ProductData productData)
    {
        int currentStock = GetCurrentStock(productData);
        int capacityPerProduct = GetProductCapacity(productData);

        if (currentStock <= (capacityPerProduct * stockThreshold))
        {
            if (!deliveringProducts.ContainsKey(productData))
            {
                int amountToRequest = capacityPerProduct - currentStock;
                TransportRequest request = CreateRequest(productData, amountToRequest);
                deliveringProducts.Add(productData, request);
            }
        }
    }

    /// <summary>
    /// Gets the storage capacity for a specific input product.
    /// Calculates capacity by dividing total input storage limit by number of input products.
    /// </summary>
    /// <param name="productData">The input product to get capacity for.</param>
    /// <returns>The storage capacity for the specified input product.</returns>
    protected override int GetProductCapacity(ProductData productData)
    {
        return baseBuilding.buildingData.inputStorageLimit / baseBuilding.buildingData.inputProducts.Length;
    }

    /// <summary>
    /// Gets the current stock level for a specific input product from the building's input stock.
    /// </summary>
    /// <param name="productData">The input product to get current stock for.</param>
    /// <returns>The current stock level for the specified input product.</returns>
    protected override int GetCurrentStock(ProductData productData)
    {
        baseBuilding.inputStock.TryGetValue(productData, out int currentStock);
        return currentStock;
    }
}