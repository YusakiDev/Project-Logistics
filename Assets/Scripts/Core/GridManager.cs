using UnityEngine;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GridManager : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public float cellSize = 1f;
    public Material lineMaterial;
    public Camera mainCamera;
    public Node[,] grid;

    void Start()
    {
        grid = new Node[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = new Node(x, y);
            }
        }

        if (mainCamera == null)
            mainCamera = Camera.main;

        GenerateGridMesh();
    }

    void GenerateGridMesh()
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> indices = new List<int>();

        float startX = -width * cellSize / 2f;
        float startY = -height * cellSize / 2f;
        float endX = startX + width * cellSize;
        float endY = startY + height * cellSize;

        // Vertical grid lines
        for (int x = 0; x <= width; x++)
        {
            float xPos = startX + x * cellSize;
            vertices.Add(new Vector3(xPos, startY, 0));
            vertices.Add(new Vector3(xPos, endY, 0));
            int idx = vertices.Count;
            indices.Add(idx - 2);
            indices.Add(idx - 1);
        }
        // Horizontal grid lines
        for (int y = 0; y <= height; y++)
        {
            float yPos = startY + y * cellSize;
            vertices.Add(new Vector3(startX, yPos, 0));
            vertices.Add(new Vector3(endX, yPos, 0));
            int idx = vertices.Count;
            indices.Add(idx - 2);
            indices.Add(idx - 1);
        }
        // Box boundary (4 lines)
        Vector3 bl = new Vector3(startX, startY, 0);
        Vector3 tl = new Vector3(startX, endY, 0);
        Vector3 tr = new Vector3(endX, endY, 0);
        Vector3 br = new Vector3(endX, startY, 0);
        int baseIdx = vertices.Count;
        vertices.Add(bl); vertices.Add(tl); // Left
        vertices.Add(tl); vertices.Add(tr); // Top
        vertices.Add(tr); vertices.Add(br); // Right
        vertices.Add(br); vertices.Add(bl); // Bottom
        for (int i = 0; i < 8; i += 2)
        {
            indices.Add(baseIdx + i);
            indices.Add(baseIdx + i + 1);
        }

        mesh.SetVertices(vertices);
        mesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);
        mesh.RecalculateBounds();

        MeshFilter mf = GetComponent<MeshFilter>();
        MeshRenderer mr = GetComponent<MeshRenderer>();
        mf.mesh = mesh;
        if (lineMaterial != null)
            mr.material = lineMaterial;
    }

    void LateUpdate()
    {
        ClampCameraToGrid();
    }

    void ClampCameraToGrid()
    {
        if (mainCamera == null) return;

        float camHeight = mainCamera.orthographicSize * 2f;
        float camWidth = camHeight * mainCamera.aspect;

        float gridWidth = width * cellSize;
        float gridHeight = height * cellSize;

        float minX = -gridWidth / 2f + camWidth / 2f;
        float maxX = gridWidth / 2f - camWidth / 2f;
        float minY = -gridHeight / 2f + camHeight / 2f;
        float maxY = gridHeight / 2f - camHeight / 2f;

        Vector3 pos = mainCamera.transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        mainCamera.transform.position = pos;
    }

    public bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    public Vector3 GetCellWorldPosition(int x, int y)
    {
        float startX = -width * cellSize / 2f;
        float startY = -height * cellSize / 2f;
        return new Vector3(startX + x * cellSize + cellSize / 2f, startY + y * cellSize + cellSize / 2f, 0);
    }
}

public class Node
{
    public int x, y;
    public NodeType type;
    public Node(int x, int y)
    {
        this.x = x;
        this.y = y;
        type = NodeType.Empty;
    }
}

public enum NodeType { Empty, Road, Building } 