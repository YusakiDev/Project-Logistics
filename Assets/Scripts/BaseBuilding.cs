using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scripts
{
    /// <summary>
    /// Base class for all buildings in the logistics game.
    /// Handles common functionality: stock management, 3D text display, and basic building behavior.
    /// 
    /// Inheritance hierarchy:
    /// - ProductionBuilding: Handles production logic (factories, farms)
    /// - Store: Handles customer sales and transport requests
    /// </summary>
    public class BaseBuilding : MonoBehaviour
    {
        #region Inspector Fields
        [FormerlySerializedAs("buildingType")]
        [Header("Building Configuration")]
        [Tooltip("Set this after instantiation! Defines the building's behavior and properties.")]
        public BuildingData buildingData;
        
        [Header("Stock Display")]
        [Tooltip("Will be auto-instantiated if null")]
        public TextMeshPro stockText3D;
        
        [Tooltip("Assign a prefab in Inspector for the stock text display")]
        public TextMeshPro stockText3DPrefab;
        
        [Tooltip("Extra offset above the building for the stock text")]
        public float textYOffset = 0.5f;
        #endregion

        #region Stock Management
        /// <summary>
        /// Products that have been delivered to this building but not yet processed.
        /// For ProductionBuilding: Raw materials waiting to be consumed
        /// For Store: Products delivered by trucks, ready to transfer to shelves
        /// </summary>
        public Dictionary<ProductData, int> inputStock = new();

        /// <summary>
        /// Products that are ready to be picked up or sold.
        /// For ProductionBuilding: Finished products ready for transport
        /// For Store: Products on shelves, ready for customer purchase
        /// </summary>
        public Dictionary<ProductData, int> outputStock = new();

        /// <summary>
        /// Stores the last recorded position of the building in the game world.
        /// Used to detect changes in position and update related elements such as the stock display text.
        /// </summary>
        private Vector3 lastPosition;

        /// <summary>
        /// Event triggered when products are delivered to the building.
        /// Provides the product type and amount delivered.
        /// </summary>
        public Action<ProductData, int> OnProductDelivered;

        #endregion

        #region Unity Lifecycle
        void Start()
        {
            EnsureStockText();
            UpdateStockTextPosition();
        }

        /// <summary>
        /// Virtual Update method
        /// that handles common building behavior.
        /// Subclasses should override this and call base.Update() first.
        /// </summary>
        protected virtual void Update()
        {
            EnsureStockText();
            
            if (Application.isPlaying)
            {
                // Early exit if the building type is not set
                if (buildingData == null) return;
                
                // Update the stock display text
                UpdateStockText();
            }

            // Check if the building position changes and update the text accordingly
            if (transform.position != lastPosition)
            {
                UpdateStockTextPosition();
                lastPosition = transform.position;
            }
        }
        #endregion

        #region Stock Text Management
        /// <summary>
        /// Ensures the stock text display is properly instantiated.
        /// Called in Start() and Update() to handle runtime instantiation.
        /// </summary>
        void EnsureStockText()
        {
            if (stockText3D == null && stockText3DPrefab != null)
            {
                stockText3D = Instantiate(stockText3DPrefab, null, true);
            }
        }

        /// <summary>
        /// Updates the 3D stock text position to float above the building.
        /// Position is calculated based on building size and grid cell size.
        /// </summary>
        void UpdateStockTextPosition()
        {
            if (stockText3D == null || buildingData == null) return;

            // Get grid cell size from GridManager
            float cellSize = 1f;

            // Calculate building height and position text above it
            float buildingHeight = buildingData.size.y * cellSize;
            Vector3 textPosition = transform.position + new Vector3(0, buildingHeight / 2f + textYOffset, 0);
            stockText3D.transform.position = textPosition;
        }

        /// <summary>
        /// Virtual method for updating the stock text display.
        /// Subclasses should override this to show building-specific information.
        /// Base implementation is empty - subclasses provide the content.
        /// </summary>
        protected virtual void UpdateStockText()
        {
            // Empty base implementation
            // Subclasses override this to display their specific stock information
        }
        #endregion

        #region Public API
        /// <summary>
        /// Adds products to the building's input stock.
        /// Used by transport system to deliver products to buildings.
        /// </summary>
        /// <param name="product">The type of product to add</param>
        /// <param name="amount">The amount to add</param>
        public void AddInputStock(ProductData product, int amount)
        {
            OnProductDelivered.Invoke(product, amount);
            if (inputStock.ContainsKey(product))
                inputStock[product] += amount;
            else
                inputStock[product] = amount;
        }

        #endregion
    }
}