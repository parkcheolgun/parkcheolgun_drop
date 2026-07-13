using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [Header("보드 크기 (프로토타입: 6x6 고정)")]
    public int width = 6;
    public int height = 6;
    public float cellSize = 1f;

    [Header("바닥 타일")]
    public GameObject tilePrefab;

    private readonly Dictionary<Vector2Int, BoardObjectController> objectMap = new();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        GenerateGridVisual();
    }

    void GenerateGridVisual()
    {
        if (tilePrefab == null) return;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 spawnPosition = GetWorldPosition(new Vector2Int(x, y));
                GameObject tile = Instantiate(tilePrefab, spawnPosition, Quaternion.identity, transform);
                tile.name = $"Tile_{x}_{y}";
            }
        }
    }

    public Vector3 GetWorldPosition(Vector2Int gridPosition)
    {
        return transform.position + new Vector3(gridPosition.x * cellSize, gridPosition.y * cellSize, 0f);
    }

    public bool IsInsideBoard(Vector2Int gridPosition)
    {
        return gridPosition.x >= 0 && gridPosition.x < width
            && gridPosition.y >= 0 && gridPosition.y < height;
    }

    public void RegisterObject(Vector2Int gridPosition, BoardObjectController obj)
    {
        objectMap[gridPosition] = obj;
    }

    public void UnregisterObject(Vector2Int gridPosition)
    {
        objectMap.Remove(gridPosition);
    }

    public bool TryGetObject(Vector2Int gridPosition, out BoardObjectController obj)
    {
        return objectMap.TryGetValue(gridPosition, out obj);
    }
}
