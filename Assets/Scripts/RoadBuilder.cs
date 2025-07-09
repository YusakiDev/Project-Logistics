using UnityEngine;
using UnityEngine.EventSystems;

public class RoadBuilder : MonoBehaviour
{
    public GridManager gridManager;
    public GameObject roadPrefab;

    public bool buildMode = false;

    // Touch tap detection
    private Vector2 touchStartPos;
    private float touchStartTime;
    public float tapMaxDuration = 0.2f;
    public float tapMaxMovement = 20f;

    void Update()
    {
        if (!buildMode) return;
#if UNITY_EDITOR
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return; // Don't build if over UI
        if (Input.GetMouseButtonDown(0)) // Left click
        {
            TryPlaceRoad(Input.mousePosition);
        }
#else
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                touchStartPos = touch.position;
                touchStartTime = Time.time;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                if (Time.time - touchStartTime < tapMaxDuration &&
                    (touch.position - touchStartPos).magnitude < tapMaxMovement)
                {
                    TryPlaceRoad(touch.position);
                }
            }
        }
#endif
    }

    void TryPlaceRoad(Vector3 screenPosition)
    {
        if (gridManager == null || roadPrefab == null) return;
        Vector3 mouseWorld = gridManager.mainCamera.ScreenToWorldPoint(screenPosition);
        int gridX = Mathf.FloorToInt((mouseWorld.x + gridManager.width * gridManager.cellSize / 2f) / gridManager.cellSize);
        int gridY = Mathf.FloorToInt((mouseWorld.y + gridManager.height * gridManager.cellSize / 2f) / gridManager.cellSize);

        if (gridManager.IsInBounds(gridX, gridY) && gridManager.grid[gridX, gridY].type != NodeType.Road)
        {
            Vector3 pos = gridManager.GetCellWorldPosition(gridX, gridY);
            Instantiate(roadPrefab, pos, Quaternion.identity);
            gridManager.grid[gridX, gridY].type = NodeType.Road;
        }
    }

    public void ToggleBuildMode()
    {
        buildMode = !buildMode;
    }
} 