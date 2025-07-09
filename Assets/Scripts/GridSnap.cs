#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[ExecuteAlways]
public class GridSnap : MonoBehaviour
{
    [Tooltip("Grid cell size for snapping")] public float cellSize = 1f;

    void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            Vector3 pos = transform.position;
            pos.x = Mathf.Round(pos.x / cellSize) * cellSize + cellSize / 2f;
            pos.y = Mathf.Round(pos.y / cellSize) * cellSize + cellSize / 2f;
            pos.z = 0;
            transform.position = pos;
        }
#endif
    }
} 