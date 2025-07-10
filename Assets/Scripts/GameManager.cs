using System.Collections.Generic;
using System.Linq;
using Scripts;
using UnityEngine;

public class GameManager : Scripts.Singleton<GameManager>
{

    public bool debugMode = true;

    public List<BaseBuilding> producerBuilding = new List<BaseBuilding>();
    public List<BaseBuilding> factoryBuilding = new List<BaseBuilding>();
    public List<BaseBuilding> storeBuilding = new List<BaseBuilding>();

    void Awake()
    {
        Screen.orientation = ScreenOrientation.AutoRotation;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Application.targetFrameRate = 60;
    }

    void Start()
    {
        var buildings = FindObjectsByType<BaseBuilding>(FindObjectsSortMode.InstanceID).ToList();

        foreach (var building in buildings)
        {
            if (building.buildingData.buildingRole == BuildingRole.Producer)
                producerBuilding.Add(building);
            else if (building.buildingData.buildingRole == BuildingRole.Factory)
                factoryBuilding.Add(building);
            else if (building.buildingData.buildingRole == BuildingRole.Store)
                storeBuilding.Add(building);
        }
    }

}

