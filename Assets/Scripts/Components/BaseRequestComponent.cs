using System;
using System.Collections.Generic;
using Scripts;
using UnityEngine;

/// <summary>
/// Abstract base class for building request components that handle stock monitoring and transport requests.
/// Provides shared functionality for timer management, request creation, and delivery tracking.
/// </summary>
public abstract class BaseRequestComponent : MonoBehaviour
{
    /// <summary>
    /// A boolean flag indicating whether the production time interval, defined in the associated
    /// <see cref="BuildingData"/> of the building, should be used as the interval for checking stock
    /// and production cycles. If set to <c>true</c>, the production time from the building's data will
    /// be used. If <c>false</c>, a custom interval defined by <see cref="customCheckInterval"/> will be used.
    /// </summary>
    public bool useProductionTimeInterval = true;

    /// <summary>
    /// A custom interval, in seconds, used for checking stock and production cycles when
    /// <see cref="useProductionTimeInterval"/> is set to <c>false</c>. This value is user-defined
    /// and overrides the default production time interval from the building's associated
    /// <see cref="BuildingData"/>.
    /// </summary>
    public float customCheckInterval = 7f;

    /// <summary>
    /// A floating-point value representing the stock level threshold at which new product delivery
    /// requests are triggered for a building. Expressed as a fraction of the total stock
    /// capacity for a given product. For example, a value of <c>0.25</c> indicates that delivery
    /// requests will be initiated when stock falls below 25% of the storage capacity for the product.
    /// </summary>
    public float stockThreshold = 0.25f;

    /// <summary>
    /// A dictionary mapping <see cref="ProductData"/> objects to their respective active transport requests.
    /// The key represents the type of product, and the value represents the active transport request
    /// that is currently being processed (including partial deliveries).
    /// </summary>
    protected Dictionary<ProductData, TransportRequest> deliveringProducts = new();

    protected float checkTimer = 0f;
    protected BaseBuilding baseBuilding;

    /// <summary>
    /// Initializes the base request component by getting the BaseBuilding component and subscribing to delivery events.
    /// </summary>
    protected virtual void Start()
    {
        baseBuilding = GetComponent<BaseBuilding>();
        baseBuilding.OnProductDelivered += OnDeliveryReceived;
    }

    /// <summary>
    /// Updates the timer and checks stock levels at defined intervals.
    /// </summary>
    protected virtual void Update()
    {
        float intervalToUse = GetCheckInterval();
        checkTimer += Time.deltaTime;

        if (checkTimer >= intervalToUse)
        {
            CheckAllProducts();
            checkTimer = 0f;
        }
    }

    /// <summary>
    /// Determines the interval to use for stock checking based on component configuration.
    /// </summary>
    /// <returns>The interval in seconds between stock checks.</returns>
    protected float GetCheckInterval()
    {
        if (useProductionTimeInterval &&
            baseBuilding.buildingData.productionTime > 0)
            return baseBuilding.buildingData.productionTime;
        else
            return customCheckInterval;
    }

    /// <summary>
    /// Creates a transport request for the specified product and amount, and tracks it for completion.
    /// </summary>
    /// <param name="productData">The product to request.</param>
    /// <param name="amount">The amount to request.</param>
    /// <returns>The created transport request.</returns>
    protected TransportRequest CreateRequest(ProductData productData, int amount)
    {
        TransportRequest request = new TransportRequest(baseBuilding, productData, amount);
        TransportManager.Instance.AddRequest(request);
        
        if (GameManager.Instance.debugMode)
        {
            Debug.Log($"{baseBuilding.name}: Item Requested {productData.name} amount {amount}");
        }
        
        return request;
    }

    /// <summary>
    /// Called when a product delivery is received. Only removes the product from the delivering products dictionary
    /// if the transport request is fully satisfied (requestingAmount <= 0).
    /// </summary>
    /// <param name="product">The product that was delivered.</param>
    /// <param name="amount">The amount that was delivered.</param>
    protected void OnDeliveryReceived(ProductData product, int amount)
    {
        if (deliveringProducts.TryGetValue(product, out TransportRequest request))
        {
            // Check if the request is fully satisfied (no more amount needed)
            if (request.requestingAmount <= 0)
            {
                deliveringProducts.Remove(product);
                if (GameManager.Instance.debugMode)
                {
                    Debug.Log($"{baseBuilding.name}: Request for {product.name} fully satisfied and removed from tracking");
                }
            }
            else
            {
                if (GameManager.Instance.debugMode)
                {
                    Debug.Log($"{baseBuilding.name}: Partial delivery for {product.name}. Still need {request.requestingAmount} more");
                }
            }
        }
    }

    /// <summary>
    /// Abstract method to check all products that this building should monitor.
    /// Must be implemented by derived classes to define which products to check.
    /// </summary>
    protected abstract void CheckAllProducts();

    /// <summary>
    /// Abstract method to check the stock level of a specific product and create requests if needed.
    /// Must be implemented by derived classes to define product-specific stock checking logic.
    /// </summary>
    /// <param name="productData">The product to check stock for.</param>
    protected abstract void CheckProductStock(ProductData productData);

    /// <summary>
    /// Abstract method to get the storage capacity for a specific product.
    /// Must be implemented by derived classes to define how capacity is calculated.
    /// </summary>
    /// <param name="productData">The product to get capacity for.</param>
    /// <returns>The storage capacity for the specified product.</returns>
    protected abstract int GetProductCapacity(ProductData productData);

    /// <summary>
    /// Abstract method to get the current stock level for a specific product.
    /// Must be implemented by derived classes to define how current stock is retrieved.
    /// </summary>
    /// <param name="productData">The product to get current stock for.</param>
    /// <returns>The current stock level for the specified product.</returns>
    protected abstract int GetCurrentStock(ProductData productData);
}