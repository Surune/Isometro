using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class Grid : MonoBehaviour
{
    public float waterLevel = .35f;
    public float scale = .1f;
    public int size = 100;

    private GameObject player;
    private List<List<Cell>> islands;

    private Cell[,] grid;
    public TileBase groundRuleTile;
    public TileBase oceanTile;
    public TileBase roadTile;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        SpawnNewMap();
        //InvokeRepeating("SpawnNewMap", 0f, 5f);
    }

    void SpawnNewMap()
    {
        GenerateNewMap();
        DrawTileMap();
        
        bool[,] visited = new bool[size, size];
        islands = new List<List<Cell>>();
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                if (grid[x, y].isWater) continue;
                
                List<Cell> island = new List<Cell>();
                CheckIsland(x, y, visited, island);
                if (island.Count > 0)
                {
                    islands.Add(island);
                }
            }
        }
        
        islands.Sort((x, y) => y.Count.CompareTo(x.Count));
        
        SpawnPlayerOnLargestIsland();
    }

    void SpawnPlayerOnLargestIsland()
    {
        var largestIsland = islands.ElementAt(0);
        var innerGround = new List<Cell>();
        foreach (Cell c in largestIsland)
        {
            if (IsInnerGround(c.x, c.y))
            {
                innerGround.Add(grid[c.x, c.y]);
            }
        }
        var spawnPoint = innerGround.ElementAt(largestIsland.Count / 2);
        // player.transform.position = new Vector3(spawnPoint.x * 4 - 198, spawnPoint.y * 4 - 198, 0);
    }
    
    void GenerateNewMap()
    {
        float[,] noiseMap = new float[size, size];
        float xOffset = Random.Range(-10000f, 10000f);
        float yOffset = Random.Range(-10000f, 10000f);
        
        float[,] fallOffMap = new float[size, size];
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float noiseValue = Mathf.PerlinNoise(x * scale + xOffset, y * scale + yOffset);
                noiseMap[x, y] = noiseValue;
                
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
                Cell cell = new Cell(x, y);
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
        Tilemap tilemapForeground = transform.GetChild(2).GetComponent<Tilemap>();
        
        tilemapGround.ClearAllTiles();
        tilemapOcean.ClearAllTiles();
        tilemapForeground.ClearAllTiles();
        
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
    }

    private bool IsWater(int x, int y)
    {
        if (x < 0 || x >= size || y < 0 || y >= size)
        {
            return true;
        }

        return grid[x, y].isWater;
    }
    
    private bool IsInnerGround(int x, int y)
    {
        return !(IsWater(x - 1, y) || IsWater(x + 1, y) || IsWater(x, y - 1) || IsWater(x, y + 1)
                 || IsWater(x - 1, y - 1) || IsWater(x - 1, y + 1) || IsWater(x + 1, y - 1) || IsWater(x + 1, y + 1));
    }
    
    private void CheckIsland(int x, int y, bool[,] visited, List<Cell> island)
    {
        // Check if the current position is out of bounds or has already been visited
        if (IsWater(x, y) || visited[x, y])
        {
            return;
        }

        // Mark the current cell as visited
        visited[x, y] = true;
        island.Add(grid[x, y]);
        
        // Recursively count the size of the island by checking neighboring cells
        CheckIsland(x + 1, y, visited, island);
        CheckIsland(x - 1, y, visited, island);
        CheckIsland(x, y + 1, visited, island);
        CheckIsland(x, y - 1, visited, island);
    }
    
    void Update()
    {
        if (Input.GetMouseButton(0))  // 왼쪽 클릭
        {
            var tilemap = transform.GetChild(0).GetComponent<Tilemap>();

            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPos = tilemap.WorldToCell(mouseWorldPos);
            var cellPosition = new Vector3Int(cellPos.x, cellPos.y, 0);
            TileBase currentTile = tilemap.GetTile(cellPosition);
            if (currentTile != null)
            {
                tilemap.SetTile(cellPosition, roadTile);  // 해당 위치에 타일 교체
                Debug.Log($"타일 교체됨: {cellPosition}");
            }
        }
    }
}
