using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class Grid : MonoBehaviour
{
    public float waterLevel = .4f;
    public float scale = .1f;
    public int size = 100;
    
    private int smallIslandThreshold = 10;

    private Cell[,] grid;
    public RuleTile groundRuleTile;
    public TileBase oceanTile;
    
    // Start is called before the first frame update
    void Start()
    {
        GenerateNewMap();
        DrawTileMap();
        
        var visited = new bool[size, size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                if (grid[x, y].isWater) continue;
                IsSmallIsland(x, y, visited);
            }
        }
    }
    
    void GenerateNewMap()
    {
        float[,] noiseMap = new float[size, size];
        float xOffset = Random.Range(-10000f, 10000f);
        float yOffset = Random.Range(-10000f, 10000f);
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float noiseValue = Mathf.PerlinNoise(x * scale + xOffset, y * scale + yOffset);
                noiseMap[x, y] = noiseValue;
            }
        }

        float[,] fallOffMap = new float[size, size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float xv = x / (float)size * 2 - 1;
                float yv = y / (float)size * 2 - 1;
                float v = Mathf.Max(Mathf.Abs(xv), Mathf.Abs(yv));
                fallOffMap[x, y] = Mathf.Pow(v, 3f) / (Mathf.Pow(v, 3f) + Mathf.Pow(2.2f - 2.2f * v, 3f));
            }
        }
        
        grid = new Cell[size, size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Cell cell = new Cell();
                float noiseValue = noiseMap[x, y] - fallOffMap[x, y];
                cell.isWater = noiseValue < waterLevel;
                grid[x, y] = cell;
            }
        }
    }

    void DrawTileMap()
    {
        Tilemap tilemapGround = transform.GetChild(0).GetComponent<Tilemap>();
        Tilemap tilemapOcean = transform.GetChild(1).GetComponent<Tilemap>();
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                if (grid[x, y].isWater)
                {
                    tilemapOcean.SetTile(new Vector3Int(x, y, 0), oceanTile);
                }
                else
                {
                    tilemapGround.SetTile(new Vector3Int(x, y, 0), groundRuleTile);
                }
            }
        }
        
        RefreshCollider(tilemapOcean);
    }
    
    void RefreshCollider (Tilemap tilemap) {
        // Update the tilemap collider by disabling and enabling the TilemapCollider2D component.
        // This is the only solution I found to refresh it during runtime.
        tilemap.gameObject.GetComponent<TilemapCollider2D>().enabled = false;
        tilemap.gameObject.GetComponent<TilemapCollider2D>().enabled = true;
    }

    bool IsWater(int x, int y)
    {
        // Check if the given cell is water (out of bounds cells are considered water)
        return x < 0 || x >= size || y < 0 || y >= size || grid[y, x].isWater;
    }
    
    public bool IsSmallIsland(int startX, int startY, bool[,] visited)
    {
        // Check if the starting point is within the grid bounds
        if (startX < 0 || startX >= size || startY < 0 || startY >= size)
        {
            throw new ArgumentException("Invalid starting point coordinates");
        }

        if (grid[startX, startY].isWater)
        {
            return false;
        }

        // Use depth-first search to traverse the island and count its size
        int islandSize = CountIslandSize(startX, startY, visited);
        Debug.Log(islandSize);
        
        return islandSize < smallIslandThreshold;
    }
    
    private int CountIslandSize(int x, int y, bool[,] visited)
    {
        // Check if the current position is out of bounds or has already been visited
        if (x < 0 || x >= size || y < 0 || y >= size || visited[x, y] || grid[x, y].isWater)
        {
            return 0;
        }

        // Mark the current cell as visited
        visited[x, y] = true;

        // Recursively count the size of the island by checking neighboring cells
        int islandSize = 1 +
                   CountIslandSize(x + 1, y, visited) +
                   CountIslandSize(x - 1, y, visited) +
                   CountIslandSize(x, y + 1, visited) +
                   CountIslandSize(x, y - 1, visited);

        return islandSize;
    }
}
