using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfficeGenAkbar : MonoBehaviour
{
    [Header("Floor Settings")]
    public float floorWidth = 100f;
    public float floorHeight = 100f;

    [Header("Grid Settings")]
    // ukuran dari grid map (width sama height nya)
    public int gridWidth = 10;
    public int gridHeight = 10;
    // setiap cell ukurannya berapa
    public float cellSizeX;
    public float cellSizeZ;
    // seed nya
    public int seed = 0;
    // apakah seednya udah pernah digunakan
    public bool usedSeed = false;

    [Header("Prefabs")]
    // list prefabnya
    // wallnya
    public GameObject wallPrefab;
    // obstacle wall
    public GameObject obstacleWallPrefab;
    // cubiclenya
    public GameObject cubiclePrefab;
    // computernya
    public GameObject computerPrefab;
    // printernya
    public GameObject fotocopyMachinePrefab;
    // floornya
    public GameObject floorPrefab;
    // player 
    public GameObject playerPrefab;
    // camera
    public Camera cam;

    // daftar dekorasinya
    public GameObject[] decorPrefabs;

    // tipe cellnya, kosong atau cubiclenya atau wallnya
    private enum CellType { Empty, Cubicle, Wall, Obstacle, NotEmpty, Door }
    // gridnya
    private CellType[,] grid;

    // Start is called before the first frame update
    void Start()
    {
        cellSizeX = floorWidth / gridWidth;
        cellSizeZ = floorHeight / gridHeight;
        cam = Camera.main;
        GenerateOffice();
    }

    private void GenerateOffice()
    {
        // kalau seednya udah pernah digunakan maka initseednya
        if (usedSeed) Random.InitState(seed);

        // bikin gridnya
        grid = new CellType[gridWidth, gridHeight];

        SpawnFloorTiles();
        SpawnOutsideWalls();
        

    }

    private void SpawnFloorTiles()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                // spawn floor per tiles
                Vector3 pos = new Vector3(x * cellSizeX + cellSizeX / 2f, 0f, z * cellSizeZ + cellSizeZ / 2f);
                GameObject tile = Instantiate(floorPrefab, pos, Quaternion.identity, transform);

                // ubah scale per tiles
                tile.transform.localScale = new Vector3(cellSizeX, 1f, cellSizeZ);
                grid[x, z] = CellType.Empty;
            }
        }
    }

    private void SpawnOutsideWalls()
    {
        int middleX = gridWidth / 2 - 1;
        int middleZ = gridHeight / 2 - 1;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                bool isEdge = (x == 0 || z == 0 || x == gridWidth - 1 || z == gridHeight - 1);
                if (isEdge)
                {
                    if(x == middleX || x == middleX + 1 || z == middleZ || z == middleZ + 1) { grid[x, z] = CellType.Door; continue; }

                    Vector3 pos = new Vector3(x * cellSizeX + cellSizeX / 2f, 4f, z * cellSizeZ + cellSizeZ / 2f);
                    GameObject wall = Instantiate(wallPrefab, pos, Quaternion.identity, transform);

                    // ubah scale per wall tiles
                    wall.transform.localScale = new Vector3(cellSizeX, 8f, cellSizeZ);
                    grid[x, z] = CellType.Wall;
                }
            }
        }
    }

    
}
