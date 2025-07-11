using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Scripts
{
    public class LevelManager : Singleton<LevelManager>
    {
        [Header("Level Configuration")]
        [Tooltip("Define overall level demand requirements")]
        public LevelDemand[] levelDemands;
        
        [Header("Customer Spawn Settings")]
        public float baseCustomerInterval = 4f;
        public float customerIntervalVariation = 2f;
        
        // Cycling system for predictable customer spawns
        private List<ProductData> demandCycle = new List<ProductData>();
        private int customerSpawnCount = 0;
        private float nextCustomerTimer;
        
        // Store management
        private List<Store> allStores = new List<Store>();
        
        public static LevelManager Instance { get; private set; }
        
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        void Start()
        {
            // Find all stores in the scene
            FindAllStores();
            
            // Generate demand cycle pattern
            GenerateDemandCycle();
            
            // Initialize customer spawn timer
            ResetCustomerTimer();
        }
        
        void Update()
        {
            // Handle customer spawning
            nextCustomerTimer -= Time.deltaTime;
            if (nextCustomerTimer <= 0f)
            {
                SpawnCustomerAtRandomStore();
                ResetCustomerTimer();
            }
        }
        
        void FindAllStores()
        {
            allStores.Clear();
            Store[] stores = FindObjectsOfType<Store>();
            allStores.AddRange(stores);
            Debug.Log($"Found {allStores.Count} stores in scene");
        }
        
        void GenerateDemandCycle()
        {
            demandCycle.Clear();
            
            if (levelDemands == null || levelDemands.Length == 0) 
            {
                Debug.Log("No level demands configured");
                return;
            }
            
            // Find the GCD to create smallest repeating pattern
            int gcd = levelDemands[0].targetAmount;
            for (int i = 1; i < levelDemands.Length; i++)
            {
                gcd = GCD(gcd, levelDemands[i].targetAmount);
            }
            
            // Create proportional cycle pattern
            foreach (var demand in levelDemands)
            {
                int timesInCycle = demand.targetAmount / gcd;
                for (int i = 0; i < timesInCycle; i++)
                {
                    demandCycle.Add(demand.product);
                }
            }
            
            Debug.Log($"Generated demand cycle with {demandCycle.Count} products. Total customers needed: {GetTotalCustomersNeeded()}");
        }
        
        int GCD(int a, int b)
        {
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }
        
        int GetTotalCustomersNeeded()
        {
            int total = 0;
            foreach (var demand in levelDemands)
            {
                total += demand.targetAmount;
            }
            return total;
        }
        
        void SpawnCustomerAtRandomStore()
        {
            if (allStores.Count == 0) return;
            
            ProductData desiredProduct = GetNextDemandProduct();
            
            if (desiredProduct != null)
            {
                // Find a store that sells this product
                Store targetStore = FindStoreForProduct(desiredProduct);
                
                if (targetStore != null)
                {
                    customerSpawnCount++;
                    targetStore.SpawnCustomerForProduct(desiredProduct);
                    Debug.Log($"Level Manager: Customer #{customerSpawnCount} sent to store wanting {desiredProduct.name}");
                }
                else
                {
                    Debug.LogWarning($"No store found that sells {desiredProduct.name}");
                }
            }
            else
            {
                Debug.Log("Level Complete! All customer demands satisfied.");
            }
        }
        
        Store FindStoreForProduct(ProductData product)
        {
            List<Store> validStores = new List<Store>();
            foreach (var store in allStores)
            {
                if (store.CanSellProduct(product))
                {
                    validStores.Add(store);
                }
            }
            
            if (validStores.Count > 0)
            {
                return validStores[Random.Range(0, validStores.Count)];
            }
            return null;
        }
        
        ProductData GetNextDemandProduct()
        {
            // Check if level is complete
            if (IsLevelComplete()) return null;
            
            // Use demand cycle if configured
            if (demandCycle.Count > 0)
            {
                int cycleIndex = customerSpawnCount % demandCycle.Count;
                return demandCycle[cycleIndex];
            }
            
            return null;
        }
        
        bool IsLevelComplete()
        {
            if (levelDemands == null || levelDemands.Length == 0) return false;
            
            foreach (var demand in levelDemands)
            {
                if (demand.currentSold < demand.targetAmount)
                {
                    return false;
                }
            }
            return true;
        }
        
        void ResetCustomerTimer()
        {
            float variation = UnityEngine.Random.Range(-customerIntervalVariation, customerIntervalVariation);
            nextCustomerTimer = baseCustomerInterval + variation;
        }
        
        // Called by stores when they serve a customer
        public void OnCustomerServed(ProductData product)
        {
            if (levelDemands == null) return;
            
            foreach (var demand in levelDemands)
            {
                if (demand.product == product)
                {
                    demand.currentSold++;
                    Debug.Log($"Level Progress: {demand.product.name} {demand.currentSold}/{demand.targetAmount}");
                    
                    // Check if level is complete
                    if (IsLevelComplete())
                    {
                        Debug.Log("🎉 LEVEL COMPLETE! All demands satisfied!");
                    }
                    break;
                }
            }
        }
        
        // Public API for other systems
        public int GetCustomerProgress(ProductData product)
        {
            if (levelDemands == null) return 0;
            
            foreach (var demand in levelDemands)
            {
                if (demand.product == product)
                {
                    return demand.currentSold;
                }
            }
            return 0;
        }
        
        public int GetCustomerTarget(ProductData product)
        {
            if (levelDemands == null) return 0;
            
            foreach (var demand in levelDemands)
            {
                if (demand.product == product)
                {
                    return demand.targetAmount;
                }
            }
            return 0;
        }
    }
    
    [Serializable]
    public class LevelDemand
    {
        public ProductData product;
        public int targetAmount;        // Total needed for level completion
        [Space]
        public int currentSold = 0;     // Track progress (managed by LevelManager)
    }
}