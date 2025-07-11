using UnityEngine;
using System.Collections.Generic;
using Scripts;
using TMPro;
using Sirenix.OdinInspector;
using Sirenix.Utilities;

/// <summary>
/// Production building handles factories and farms that produce goods.
/// Extends BaseBuilding with production logic, timers, and capacity management.
/// 
/// Production Flow:
/// 1. Check if can produce (has inputs, output not full)
/// 2. Consume inputs immediately when production starts
/// 3. Wait for production time to complete
/// 4. Add output products to outputStock
/// 5. Ready for transport system to pick up
/// </summary>
public class ProductionBuilding : BaseBuilding
{
    #region Production State
    /// <summary>
    /// Current production timer. Tracks progress toward completion.
    /// </summary>
    private float productionTimer = 0f;
    
    /// <summary>
    /// Whether this building is currently producing.
    /// Prevents starting new production cycles while one is active.
    /// </summary>
    private bool isProducing = false;
    #endregion

    #region Experimental Testing Features
    [Header("Experimental Delivery Testing")]
    [Tooltip("Enable experimental delivery simulation for testing")]
    public bool experimental;

    [ShowIf("experimental")]
    [Tooltip("Product type to simulate deliveries for testing")]
    public ProductData deliveredProduct;

    [ShowIf("experimental")]
    [Tooltip("How often to simulate deliveries (seconds)")]
    public float deliveryInterval = 5f;
    
    /// <summary>
    /// Timer for experimental delivery simulation.
    /// TODO: Remove when real transport system is implemented.
    /// </summary>
    private float deliveryTimer = 0f;
    #endregion

    #region Unity Lifecycle
    protected override void Update()
    {
        base.Update(); // Call base class Update first
        
        if (Application.isPlaying)
        {
            if (buildingData == null) return;

            // Handle production cycle
            ProcessProductionCycle();
            
            // Handle experimental delivery system (temporary)
            if (experimental)
            {
                HandleExperimentalDelivery();
            }
        }
    }
    #endregion

    #region Production Logic
    /// <summary>
    /// Processes the complete production cycle:
    /// 1. Start production if possible
    /// 2. Update production timer
    /// 3. Complete production when timer reaches target
    /// </summary>
    private void ProcessProductionCycle()
    {
        // Start production if not currently producing and can produce
        if (!isProducing && CanProduce())
        {
            StartProduction();
        }

        // Update production timer if currently producing
        if (isProducing)
        {
            UpdateProductionTimer();
        }
    }

    /// <summary>
    /// Starts a new production cycle by consuming inputs immediately.
    /// This follows the design: inputs consumed at start, outputs added at end.
    /// </summary>
    private void StartProduction()
    {
        // Consume input immediately when production starts
        foreach (var input in buildingData.inputProducts)
        {
            inputStock[input.productData] -= input.amount;
        }
        
        isProducing = true;
        productionTimer = 0f;
    }

    /// <summary>
    /// Updates the production timer and completes production when time is reached.
    /// </summary>
    private void UpdateProductionTimer()
    {
        productionTimer += Time.deltaTime;
        
        if (productionTimer >= buildingData.productionTime)
        {
            CompleteProduction();
        }
    }

    /// <summary>
    /// Completes the production cycle by adding output products.
    /// </summary>
    private void CompleteProduction()
    {
        Produce();
        productionTimer = 0f;
        isProducing = false;
    }

    /// <summary>
    /// Checks if this building can start production.
    /// Requirements:
    /// - Output storage not full
    /// - Has required input materials (if any)
    /// </summary>
    /// <returns>True if production can start</returns>
    private bool CanProduce()
    {
        // Check if output storage is full
        outputStock.TryGetValue(buildingData.outputProduct, out int currentOutputStock);
        if (currentOutputStock >= buildingData.outputStorageLimit)
            return false;

        // Check if inputs are available (farms don't need inputs)
        if (buildingData.inputProducts == null || buildingData.inputProducts.Length == 0)
            return true; // No input required (e.g., farm)

        // Verify all required inputs are available
        foreach (var input in buildingData.inputProducts)
        {
            if (input.productData == null) // Safety check
                return false;
                
            if (!inputStock.TryGetValue(input.productData, out int currentInputStock) ||
                currentInputStock < input.amount)
                return false;
        }
        
        return true;
    }

    /// <summary>
    /// Produces the output product and adds it to output stock.
    /// Called when production timer completes.
    /// </summary>
    private void Produce()
    {
        if (buildingData.outputProduct != null)
        {
            if (!outputStock.ContainsKey(buildingData.outputProduct))
                outputStock[buildingData.outputProduct] = 0;
                
            outputStock[buildingData.outputProduct] += buildingData.outputAmount;
        }
        
        // TODO: Add events/notifications for when production happens
        // This could trigger transport requests or other game systems
    }
    #endregion

    #region Experimental Delivery System
    /// <summary>
    /// Handles experimental delivery arrivals for testing.
    /// TODO: Remove this when real transport system is implemented.
    /// </summary>
    private void HandleExperimentalDelivery()
    {
        if (deliveredProduct == null) return;

        // Check if input storage has capacity
        inputStock.TryGetValue(deliveredProduct, out int currentInputStock);
        int inputCapacityPerProduct = buildingData.inputStorageLimit / buildingData.inputProducts.Length;
        
        if (currentInputStock >= inputCapacityPerProduct)
        {
            return; // Storage full
        }

        // Simulate delivery arrivals
        deliveryTimer += Time.deltaTime;
        if (deliveryTimer >= deliveryInterval)
        {
            AddInputStock(deliveredProduct, 1);
            deliveryTimer = 0f;
        }
    }
    #endregion

    #region Display
    /// <summary>
    /// Updates the stock text display with production-specific information.
    /// Shows production timer, input/output stocks, and experimental delivery info.
    /// </summary>
    protected override void UpdateStockText()
    {
        if (stockText3D == null) return;
        
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        
        // Show production timer in debug mode
        if (GameManager.Instance.debugMode)
        {
            sb.Append($"Time: {productionTimer:F2} / {buildingData.productionTime}");
            sb.AppendLine();
        }
        
        // Show input stock levels
        DisplayInputStock(sb);
        
        // Show output stock levels
        DisplayOutputStock(sb);
        
        // Show transport requests
        DisplayTransportStatus(sb);
        
        // Show experimental delivery info
        if (experimental && deliveredProduct != null)
        {
            sb.Append($"Next delivery in: {deliveryInterval - deliveryTimer:F2}s");
        }
        
        stockText3D.text = sb.ToString();
    }

    /// <summary>
    /// Displays input stock information in the text display.
    /// </summary>
    private void DisplayInputStock(System.Text.StringBuilder sb)
    {
        if (buildingData.inputProducts != null && buildingData.inputProducts.Length > 0)
        {
            sb.Append("Input: ");
            
            for (int i = 0; i < buildingData.inputProducts.Length; i++)
            {
                var prod = buildingData.inputProducts[i];
                int have = inputStock.ContainsKey(prod.productData) ? inputStock[prod.productData] : 0;
                int capacity = buildingData.inputStorageLimit / buildingData.inputProducts.Length;
                
                sb.Append($"{prod.productData.productName} {have} / {capacity}");
                
                if (i < buildingData.inputProducts.Length - 1)
                    sb.Append(", ");
            }
            
            sb.AppendLine();
        }
    }

    /// <summary>
    /// Displays output stock information in the text display.
    /// </summary>
    private void DisplayOutputStock(System.Text.StringBuilder sb)
    {
        if (buildingData.outputProduct != null)
        {
            sb.Append("Output: ");
            int have = outputStock.ContainsKey(buildingData.outputProduct) ? outputStock[buildingData.outputProduct] : 0;
            sb.Append($"{buildingData.outputProduct.productName} {have} / {buildingData.outputStorageLimit}");
            sb.AppendLine();
        }
    }
    
    private void DisplayTransportStatus(System.Text.StringBuilder sb)
    {
        // Count active deliveries for this building
        int incomingDeliveries = 0;
        
        if (TransportManager.Instance != null)
        {
            foreach (var delivery in TransportManager.Instance.GetActiveDeliveries())
            {
                if (delivery.requester == this)
                {
                    incomingDeliveries++;
                }
            }
        }
        
        if (incomingDeliveries > 0)
        {
            sb.Append($" {incomingDeliveries} truck(s) en route");
            sb.AppendLine();
        }
    }
    #endregion
} 