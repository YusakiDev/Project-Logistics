using System;
using System.Collections.Generic;
using System.Linq;
using Scripts;
using UnityEngine;

public class BuildingRequestComponent : MonoBehaviour
{
    public bool useProductionTimeInterval = true;
    public float customCheckInterval = 7f;
    public float stockThreshold = 0.25f;

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
        Debug.Log($"Item Requested {productData} amount {amount}");
    }

    void OnDeliveryReceived(ProductData product, int amount)
    {
        deliveringProducts.Remove(product);
    }
}