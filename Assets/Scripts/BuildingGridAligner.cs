using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(ProductionBuilding))]
public class BuildingGridAligner : MonoBehaviour
{
    private Vector3 lastSnappedPosition;

    void Update()
    {
        
        if (transform.position != lastSnappedPosition)
        {
            SnapAndOffsetToGrid();
            lastSnappedPosition = transform.position;
        }
        
    }

    private void SnapAndOffsetToGrid()
    {
        var buildingInstance = GetComponent<ProductionBuilding>();
        if (buildingInstance == null || buildingInstance.buildingData == null)
            return;

        float cellSize = 1f;
        GridManager gridManager = FindObjectOfType<GridManager>();
        if (gridManager != null)
            cellSize = gridManager.cellSize;
        Vector2Int size = buildingInstance.buildingData.size;
        transform.localScale = new Vector3(size.x * cellSize, size.y * cellSize, 1f);

        // Snap to bottom-left of grid
        Vector3 pos = transform.position;
        pos.x = Mathf.Round(pos.x / cellSize) * cellSize;
        pos.y = Mathf.Round(pos.y / cellSize) * cellSize;

        // Offset so the CENTER of the sprite is at the center of the area it covers
        pos.x += (size.x * cellSize) / 2f;
        pos.y += (size.y * cellSize) / 2f;
        pos.z = 0;
        transform.position = pos;
    }
} 