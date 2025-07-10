using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Scripts
{
    public class BaseBuilding : MonoBehaviour
    {
        [Header("Set this after instantiation!")]
        public BuildingType buildingType;
        public TextMeshPro stockText3D; // Will be auto-instantiated if null
        public TextMeshPro stockText3DPrefab; // Assign a prefab in Inspector
        public float textYOffset = 0.5f;  // Extra offset above the building for the stock text

        public Dictionary<ProductType, int> inputStock = new();

        public Dictionary<ProductType, int> outputStock = new();
    }
}