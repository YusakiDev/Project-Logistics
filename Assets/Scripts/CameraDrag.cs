using UnityEngine;

public class CameraDrag : MonoBehaviour
{
    public Camera cam;
    private Vector3 lastPanPosition;
    private int panFingerId; // Touch mode only
    private bool isPanning;
    public float dragThreshold = 10f; // Minimum pixels to start panning

    void Awake()
    {
        if (cam == null) cam = Camera.main;
    }

    void Update()
    {
#if UNITY_EDITOR
        // Mouse controls for Editor
        if (Input.GetMouseButtonDown(0))
        {
            lastPanPosition = Input.mousePosition;
            isPanning = false;
        }
        else if (Input.GetMouseButton(0))
        {
            if (!isPanning && (Input.mousePosition - lastPanPosition).magnitude > dragThreshold)
                isPanning = true;
            if (isPanning)
                PanCamera(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isPanning = false;
        }
#else
        // Touch controls for device
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                lastPanPosition = touch.position;
                panFingerId = touch.fingerId;
                isPanning = false;
            }
            else if (touch.fingerId == panFingerId && (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary))
            {
                if (!isPanning && (touch.position - (Vector2)lastPanPosition).magnitude > dragThreshold)
                    isPanning = true;
                if (isPanning)
                    PanCamera(touch.position);
            }
            else if (touch.fingerId == panFingerId && (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled))
            {
                isPanning = false;
            }
        }
#endif
    }

    void PanCamera(Vector3 newPanPosition)
    {
        Vector3 offset = cam.ScreenToWorldPoint(lastPanPosition) - cam.ScreenToWorldPoint(newPanPosition);
        offset.z = 0;
        cam.transform.position += offset;
        lastPanPosition = newPanPosition;
    }
} 