using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class CustomerData
    {
        public ProductData productData;
        public float timeWaited;

        public CustomerData(ProductData productData, float timeWaited)
        {
            this.productData = productData;
            this.timeWaited = timeWaited;
        }
    }

    public class Store : BaseBuilding
    {
        public Dictionary<ProductData, int> storeStock = new();
        public Dictionary<float, CustomerData> customerQueue = new();

        [Header("Customer Settings")]
        public float timeOut = 30f;
        
        [Header("Service Intervals")]
        public float customerBuyInterval = 0.5f;
        private float customerBuyTimer;

        void Start()
        {
            foreach (var stock in buildingData.storeStock)
            {
                storeStock.Add(stock.productData, 0);
            }
            
            // Initialize service timer
            customerBuyTimer = customerBuyInterval;
        }

        protected override void Update()
        {
            base.Update();

            // Handle customer service
            customerBuyTimer -= Time.deltaTime;
            if (customerBuyTimer <= 0)
            {
                CustomerBuyItem();
                customerBuyTimer = customerBuyInterval;
            }

            UpdateCustomerQueue();
        }

        void CustomerBuyItem()
        {
            // Find first customer (oldest) and try to serve them
            if (customerQueue.Count == 0) return;
            
            float oldestCustomerTime = float.MaxValue;
            foreach (var customer in customerQueue)
            {
                if (customer.Key < oldestCustomerTime)
                {
                    oldestCustomerTime = customer.Key;
                }
            }
            
            // Try to serve the oldest customer
            if (TryServeCustomer(oldestCustomerTime))
            {
                customerQueue.Remove(oldestCustomerTime);
            }
        }
        
        bool TryServeCustomer(float customerTime)
        {
            if (!customerQueue.ContainsKey(customerTime)) return false;
            
            CustomerData customer = customerQueue[customerTime];
            ProductData wantedProduct = customer.productData;
            
            // Check if the specific product they want is in stock
            if (storeStock.ContainsKey(wantedProduct) && storeStock[wantedProduct] > 0)
            {
                // Serve the customer with their desired product
                storeStock[wantedProduct]--;
                
                // Notify LevelManager about the sale
                if (LevelManager.Instance != null)
                {
                    LevelManager.Instance.OnCustomerServed(wantedProduct);
                }
                
                Debug.Log($"Customer served with {wantedProduct.name}. Remaining stock: {storeStock[wantedProduct]}");
                return true; // Customer successfully served
            }
            
            return false; // Their specific product not available
        }

        void UpdateCustomerQueue()
        {
            List<float> customersToRemove = new List<float>();
            foreach (var customer in customerQueue)
            {
                customerQueue[customer.Key].timeWaited += Time.deltaTime;
                if (customerQueue[customer.Key].timeWaited >= timeOut)
                {
                    customersToRemove.Add(customer.Key);
                    Debug.Log($"Customer left {name} after waiting {timeOut} seconds for {customer.Value.productData.name}");
                }
            }

            foreach (var customerTime in customersToRemove)
            {
                customerQueue.Remove(customerTime);
            }
        }

        // Called by LevelManager to spawn a customer wanting a specific product
        public void SpawnCustomerForProduct(ProductData desiredProduct)
        {
            float arrivalTime = Time.time;
            customerQueue.Add(arrivalTime, new CustomerData(desiredProduct, 0f));
            Debug.Log($"Customer arrived at {name} wanting {desiredProduct.name}");
        }

        // Check if this store can sell a specific product
        public bool CanSellProduct(ProductData product)
        {
            return storeStock.ContainsKey(product);
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
            
            // Add detailed customer queue status
            if (customerQueue.Count > 0)
            {
                Dictionary<ProductData, int> queueCounts = new Dictionary<ProductData, int>();
                
                // Count how many customers want each product
                foreach (var customer in customerQueue)
                {
                    ProductData wantedProduct = customer.Value.productData;
                    if (queueCounts.ContainsKey(wantedProduct))
                        queueCounts[wantedProduct]++;
                    else
                        queueCounts.Add(wantedProduct, 1);
                }
                
                // Build display string
                List<string> queueDetails = new List<string>();
                foreach (var count in queueCounts)
                {
                    queueDetails.Add($"{count.Value}×{count.Key.name}");
                }
                
                sb.AppendLine($" {customerQueue.Count} waiting: {string.Join(", ", queueDetails)}");
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