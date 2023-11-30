using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class Grid : MonoBehaviour
{
    public Material terrainMaterial;
    public Material edgeMaterial;
    public float waterLevel = .4f;
    public float scale = .1f;
    public int size = 100;
    
    private int smallIslandThreshold = 10;

    private Cell[,] grid;
    public TileBase s;
    
    // Start is called before the first frame update
    void Start()
    {
        GenerateNewMap();
        //DrawTerrainMesh(grid);
        //DrawEdgeMesh(grid);
        //DrawTexture(grid);
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
                float xv = x / (float)size*2-1;
                float yv = y / (float)size*2-1;
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
        Tilemap tilemap = gameObject.GetComponentInChildren<Tilemap>();
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                if (grid[x, y].isWater) continue;
                tilemap.SetTile(new Vector3Int(x, y, 0), s);
            }
        }
    }
    
    void DrawTerrainMesh(Cell[,] grid)
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Cell cell = grid[x, y];
                Vector3 a = new Vector3(x - .5f, y + .5f, 0);
                Vector3 b = new Vector3(x + .5f, y + .5f, 0);
                Vector3 c = new Vector3(x - .5f, y - .5f, 0);
                Vector3 d = new Vector3(x + .5f, y - .5f, 0);
                Vector2 uvA = new Vector2(x / (float)size, y / (float)size);
                Vector2 uvB = new Vector2((x + 1) / (float)size, y / (float)size);
                Vector2 uvC = new Vector2(x / (float)size, (y + 1) / (float)size);
                Vector2 uvD = new Vector2((x + 1) / (float)size, (y + 1) / (float)size);
                Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                Vector2[] uv = new Vector2[] { uvA, uvB, uvC, uvB, uvD, uvC };
                for (int k = 0; k < 6; k++)
                {
                    vertices.Add(v[k]);
                    triangles.Add(triangles.Count);
                    uvs.Add(uv[k]);
                }
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;
    }
    
    void DrawEdgeMesh(Cell[,] grid) {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        for(int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Cell cell = grid[x, y];
                if(!cell.isWater) {
                    if(x > 0) {
                        Cell left = grid[x - 1, y];
                        if(left.isWater)
                        {
                            Vector3 a = new Vector3(x - .5f, y + .5f, 0);
                            Vector3 b = new Vector3(x - .5f, y - .5f, 0);
                            Vector3 c = new Vector3(x - .5f, y + .5f, -1);
                            Vector3 d = new Vector3(x - .5f, y - .5f, -1);
                            Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                            for(int k = 0; k < 6; k++)
                            {
                                vertices.Add(v[k]);
                                triangles.Add(triangles.Count);
                            }
                        }
                    }
                    if(x < size - 1)
                    {
                        Cell right = grid[x + 1, y];
                        if(right.isWater)
                        {
                            Vector3 a = new Vector3(x + .5f, y - .5f, 0);
                            Vector3 b = new Vector3(x + .5f, y + .5f, 0);
                            Vector3 c = new Vector3(x + .5f, y - .5f, -1);
                            Vector3 d = new Vector3(x + .5f, y + .5f, -1);
                            Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                            for(int k = 0; k < 6; k++)
                            {
                                vertices.Add(v[k]);
                                triangles.Add(triangles.Count);
                            }
                        }
                    }
                    if(y > 0)
                    {
                        Cell down = grid[x, y - 1];
                        if(down.isWater)
                        {
                            Vector3 a = new Vector3(x - .5f, y - .5f, 0);
                            Vector3 b = new Vector3(x + .5f, y - .5f, 0);
                            Vector3 c = new Vector3(x - .5f, y - .5f, -1);
                            Vector3 d = new Vector3(x + .5f, y - .5f, -1);
                            Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                            for(int k = 0; k < 6; k++)
                            {
                                vertices.Add(v[k]);
                                triangles.Add(triangles.Count);
                            }
                        }
                    }
                    if(y < size - 1)
                    {
                        Cell up = grid[x, y + 1];
                        if(up.isWater)
                        {
                            Vector3 a = new Vector3(x + .5f, y + .5f, 0);
                            Vector3 b = new Vector3(x - .5f, y + .5f, 0);
                            Vector3 c = new Vector3(x + .5f, y + .5f, -1);
                            Vector3 d = new Vector3(x - .5f, y + .5f, -1);
                            Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                            for(int k = 0; k < 6; k++)
                            {
                                vertices.Add(v[k]);
                                triangles.Add(triangles.Count);
                            }
                        }
                    }
                }
            }
        }
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        GameObject edgeObj = new GameObject("Edge");
        edgeObj.transform.SetParent(transform);

        MeshFilter meshFilter = edgeObj.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshRenderer meshRenderer = edgeObj.AddComponent<MeshRenderer>();
        meshRenderer.material = edgeMaterial;
    }

    void DrawTexture(Cell[,] grid)
    {
        Texture2D texture = new Texture2D(size, size);
        Color[] colorMap = new Color[size * size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Cell cell = grid[x, y];
                if (cell.isWater)
                {
                    colorMap[y * size + x] = Color.blue;
                }
                else
                {
                    colorMap[y * size + x] = Color.green;
                }
            }
        }

        texture.filterMode = FilterMode.Point;
        texture.SetPixels(colorMap);
        texture.Apply();

        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        terrainMaterial.mainTexture = texture;
        meshRenderer.material = terrainMaterial;
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
