using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class Store : BaseBuilding
    {
        public Dictionary<ProductData, int> storeStock  = new();

        void Start()
        {
            foreach (var stock in buildingData.storeStock)
            {
                storeStock.Add(stock.productData,  0);
            }
        }

        public void AddStoreStock(ProductData product, int amount)
        {
            if (storeStock.ContainsKey(product))
                storeStock[product] += amount;
            else
                storeStock.Add(product, amount);
                
            // Trigger delivery event to notify request components
            OnProductDelivered?.Invoke(product, amount);
        }

        protected override void UpdateStockText()
        {
            base.UpdateStockText();
            
            // Add store-specific stock display
            System.Text.StringBuilder sb = new();
            
            foreach (var stock in storeStock)
            {
                // Find the max capacity for this product
                int maxCapacity = 0;
                foreach (var storeProduct in buildingData.storeStock)
                {
                    if (storeProduct.productData == stock.Key)
                    {
                        maxCapacity = storeProduct.amount;
                        break;
                    }
                }
                
                sb.AppendLine($" {stock.Key.name}: {stock.Value}/{maxCapacity}");
            }
            
            // Add truck enroute status
            DisplayTransportStatus(sb);

            if (stockText3D != null)
            {
                stockText3D.text = sb.ToString();
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
                sb.AppendLine($" {incomingDeliveries} truck(s) en route");
            }
        }
    }

}