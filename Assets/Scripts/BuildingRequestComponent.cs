using System;
using System.Collections.Generic;
using System.Linq;
using Scripts;
using UnityEngine;

public class BuildingRequestComponent : MonoBehaviour
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
    /// requests are triggered for a building. Expressed as a fraction of the total input stock
    /// capacity for a given product. For example, a value of <c>0.25</c> indicates that delivery
    /// requests will be initiated when stock falls below 25% of the storage capacity for the product.
    /// </summary>
    public float stockThreshold = 0.25f;

    /// <summary>
    /// A dictionary mapping <see cref="ProductData"/> objects to their respective quantities that are currently
    /// being delivered by the building. The key represents the type of product, and the value represents the
    /// amount of that product being transported or prepared for delivery.
    /// </summary>
    Dictionary<ProductData, int> deliveringProducts = new();

    private float checkTimer = 0f;
    private BaseBuilding baseBuilding;

    private void Start()
    {
        baseBuilding = GetComponent<BaseBuilding>();
        baseBuilding.OnProductDelivered += OnDeliveryReceived;
    }

    private void Update()
    {
        float intervalToUse = GetCheckInterval();
        checkTimer += Time.deltaTime;


        if (checkTimer >= intervalToUse) {
            foreach (var product in baseBuilding.buildingData.inputProducts)
            {
                CheckProductStock(product.productData);
            }
            checkTimer = 0f;
        }

    }

    float GetCheckInterval()
    {
        if (useProductionTimeInterval &&
            baseBuilding.buildingData.productionTime > 0)
            return baseBuilding.buildingData.productionTime;
        else
            return customCheckInterval;
    }

    void CheckProductStock(ProductData productData)
    {
        baseBuilding.inputStock.TryGetValue(productData,  out int currentStock);
        int capacityPerProduct = baseBuilding.buildingData.inputStorageLimit
                                 / baseBuilding.buildingData.inputProducts.Length;

        if (currentStock <= (capacityPerProduct * stockThreshold))
        {
            if (!deliveringProducts.ContainsKey(productData))
            {
                deliveringProducts.Add(productData, capacityPerProduct - currentStock);
                CreateRequest(productData,  capacityPerProduct - currentStock); // 1 for now
            }
        }
    }

    void CreateRequest(ProductData productData, int amount)
    {
        TransportManager.Instance.AddRequest(new TransportRequest(baseBuilding, productData, amount));
        Debug.Log($"Item Requested {productData} amount {amount}");
    }

    void OnDeliveryReceived(ProductData product, int amount)
    {
        deliveringProducts.Remove(product);
    }
}