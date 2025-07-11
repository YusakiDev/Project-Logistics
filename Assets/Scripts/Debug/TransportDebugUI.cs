using System.Collections.Generic;
using System.Text;
using Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Debug UI component that displays all active transport requests and deliveries.
/// Only shows when GameManager debug mode is enabled.
/// </summary>
public class TransportDebugUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI requestsText;
    public GameObject debugPanel;
    
    [Header("Update Settings")]
    public float updateInterval = 0.5f;
    
    private float updateTimer = 0f;
    private StringBuilder sb = new StringBuilder();
    
    void Start()
    {
        // Initially hide the debug panel
        if (debugPanel != null)
        {
            debugPanel.SetActive(false);
        }
    }
    
    void Update()
    {
        // Only show debug UI when debug mode is enabled
        bool shouldShow = GameManager.Instance != null && GameManager.Instance.debugMode;
        
        if (debugPanel != null && debugPanel.activeSelf != shouldShow)
        {
            debugPanel.SetActive(shouldShow);
        }
        
        if (!shouldShow || requestsText == null || TransportManager.Instance == null)
            return;
            
        // Update the display at specified intervals
        updateTimer += Time.deltaTime;
        if (updateTimer >= updateInterval)
        {
            updateTimer = 0f;
            UpdateDebugDisplay();
        }
    }
    
    void UpdateDebugDisplay()
    {
        sb.Clear();
        sb.AppendLine("=== TRANSPORT DEBUG ===");
        sb.AppendLine();
        
        // Show pending requests
        var pendingRequests = GetPendingRequests();
        sb.AppendLine($"PENDING REQUESTS ({pendingRequests.Count}):");
        foreach (var request in pendingRequests)
        {
            sb.AppendLine($" {request.requestingBuilding.name} → {request.requestingProduct.name} ({request.requestingAmount})");
        }
        
        if (pendingRequests.Count == 0)
        {
            sb.AppendLine("  None");
        }
        
        sb.AppendLine();
        
        // Show active deliveries
        var activeDeliveries = TransportManager.Instance.GetActiveDeliveries();
        sb.AppendLine($"ACTIVE DELIVERIES ({activeDeliveries.Count}):");
        foreach (var delivery in activeDeliveries)
        {
            string phaseIcon = delivery.deliveryPhase == DeliveryPhase.Pickup ? "P" : "D";
            string phase = delivery.deliveryPhase.ToString();
            int timeLeft = Mathf.CeilToInt(delivery.timeRemaining);
            
            sb.AppendLine($"{phaseIcon} {delivery.supplier.name} → {delivery.requester.name}");
            sb.AppendLine($"    {delivery.productData.name} ({delivery.amount}) | {phase} ({timeLeft}s)");
        }
        
        if (activeDeliveries.Count == 0)
        {
            sb.AppendLine("  None");
        }
        
        sb.AppendLine();
        
        // Show request summary by building
        sb.AppendLine("REQUEST SUMMARY:");
        var requestSummary = GetRequestSummaryByBuilding();
        foreach (var kvp in requestSummary)
        {
            sb.AppendLine($" {kvp.Key}: {kvp.Value} active requests");
        }
        
        if (requestSummary.Count == 0)
        {
            sb.AppendLine("  No active requests");
        }
        
        requestsText.text = sb.ToString();
    }
    
    /// <summary>
    /// Gets pending requests from TransportManager
    /// </summary>
    List<TransportRequest> GetPendingRequests()
    {
        return TransportManager.Instance.GetPendingRequests();
    }
    
    /// <summary>
    /// Creates a summary of how many requests each building has
    /// </summary>
    Dictionary<string, int> GetRequestSummaryByBuilding()
    {
        var summary = new Dictionary<string, int>();
        var activeDeliveries = TransportManager.Instance.GetActiveDeliveries();
        
        foreach (var delivery in activeDeliveries)
        {
            string buildingName = delivery.requester.name;
            if (summary.ContainsKey(buildingName))
            {
                summary[buildingName]++;
            }
            else
            {
                summary[buildingName] = 1;
            }
        }
        
        return summary;
    }
}