using UnityEngine;
using System.Collections.Generic;
using TMPro;
using Sirenix.OdinInspector;

public class BuildingInstance : MonoBehaviour
{
    [Header("Set this after instantiation!")]
    public BuildingType buildingType;
    public TextMeshPro stockText; // Assign in Inspector or via code

    // Per-instance state
    private float productionTimer = 0f;
    public Dictionary<ProductType, int> inputStock = new();
    public Dictionary<ProductType, int> outputStock = new();

    public bool experimental;


    // Delivery logic
    [ShowIf("experimental")]
    public ProductType deliveredProduct; // Assign in Inspector (e.g., Cake)

    [ShowIf("experimental")]
    public float deliveryInterval = 5f;
    private float deliveryTimer = 0f;

    private bool isProducing = false;

    void OnValidate()
    {
        SpriteRenderer icon = GetComponent<SpriteRenderer>();
        icon.sprite = buildingType.icon;
    }

    void Update()
    {
        if (buildingType == null) return;

        if (!isProducing && CanProduce())
        {
            // Consume input immediately when production starts
            foreach (var input in buildingType.inputProducts)
            {
                inputStock[input.productType] -= input.amount;
            }
            isProducing = true;
            productionTimer = 0f; // Reset timer to 0
        }

        if (isProducing)
        {
            productionTimer += Time.deltaTime;
            if (productionTimer >= buildingType.productionTime)
            {
                Produce();
                productionTimer = 0f;
                isProducing = false; // Reset production flag
            }
        }
        HandleDeliveryArrival();
        UpdateStockText();
    }

    void HandleDeliveryArrival()
    {
        if (deliveredProduct == null) return;
        deliveryTimer += Time.deltaTime;
        if (deliveryTimer >= deliveryInterval)
        {
            AddInputStock(deliveredProduct, 1); // Add 1 to input stock
            deliveryTimer = 0f;
        }
    }

    // Check if building has enough input to produce
    bool CanProduce()
    {
        if (buildingType.inputProducts == null || buildingType.inputProducts.Length == 0)
            return true; // No input required (e.g., farm)
        foreach (var input in buildingType.inputProducts)
        {
            if (input.productType == null) // <-- Add this check
                return false;
            if (!inputStock.TryGetValue(input.productType, out int have) || have < input.amount)
                return false;
        }
        return true;
    }

    // Consume input and add output
    void Produce()
    {
        // Add output
        if (buildingType.outputProduct != null)
        {
            if (!outputStock.ContainsKey(buildingType.outputProduct))
                outputStock[buildingType.outputProduct] = 0;
            outputStock[buildingType.outputProduct] += buildingType.outputAmount;
        }
        // You can add events/logic here for when production happens
    }

    // Update the stock text display
    void UpdateStockText()
    {
        if (stockText == null) return;
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        if (GameManager.Instance.debugMode){
            sb.Append($"Time: {productionTimer:F2} / {buildingType.productionTime}");
            sb.AppendLine();
        }
        if (buildingType.inputProducts != null && buildingType.inputProducts.Length > 0)
        {
            sb.Append("Input: ");
            for (int i = 0; i < buildingType.inputProducts.Length; i++)
            {
                var prod = buildingType.inputProducts[i];
                int have = inputStock.ContainsKey(prod.productType) ? inputStock[prod.productType] : 0;
                sb.Append($"{prod.productType.productName} {have}");
                if (i < buildingType.inputProducts.Length - 1) sb.Append(", ");
            }
            sb.AppendLine();
        }
        if (buildingType.outputProduct != null)
        {
            sb.Append("Output: ");
            int have = outputStock.ContainsKey(buildingType.outputProduct) ? outputStock[buildingType.outputProduct] : 0;
            sb.Append($"{buildingType.outputProduct.productName} {have}");
            sb.AppendLine();
        }
        if (deliveredProduct != null)
        {
            sb.Append($"Next delivery in: {deliveryInterval - deliveryTimer:F2}s");
        }
        stockText.text = sb.ToString();
    }

    public void AddInputStock(ProductType product, int amount)
    {
        if (inputStock.ContainsKey(product))
            inputStock[product] += amount;
        else
            inputStock[product] = amount;
    }
} 