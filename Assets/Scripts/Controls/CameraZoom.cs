using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public Camera cam;
    public float zoomSpeed = 0.5f;
    public float minZoom = 3f;
    public float maxZoom = 20f;

    void Awake()
    {
        if (cam == null) cam = Camera.main;
    }

    void Update()
    {
#if UNITY_EDITOR
        // Mouse wheel zoom for editor/desktop
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - scroll * zoomSpeed * 10f, minZoom, maxZoom);
        }
#else
        // Pinch-to-zoom for mobile
        if (Input.touchCount == 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            Vector2 prev0 = t0.position - t0.deltaPosition;
            Vector2 prev1 = t1.position - t1.deltaPosition;

            float prevDist = (prev0 - prev1).magnitude;
            float currDist = (t0.position - t1.position).magnitude;

            float delta = currDist - prevDist;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - delta * zoomSpeed * Time.deltaTime, minZoom, maxZoom);
        }
#endif
    }
}