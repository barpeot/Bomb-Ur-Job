using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfficeGenerator : MonoBehaviour
{
    [Header("Grid Settings")]
    // ukuran dari mapnya (width sama height nya)
    public int width = 10;
    public int height = 10;
    // setiap cell ukurannya berapa
    public float cellSize = 2f;
    // seed nya
    public int seed = 0;
    // apakah seednya udah pernah digunakan
    public bool usedSeed = false;

    [Header("Prefabs")]
    // list prefabnya
    // wallnya
    public GameObject wallVerticalPrefab;
    public GameObject wallHorizontalPrefab;
    // cubiclenya
    public GameObject cubiclePrefab;
    // computernya
    public GameObject computerPrefab;
    // printernya
    public GameObject fotocopyMachinePrefab;
    // floornya
    public GameObject floorPrefab;
    // daftar dekorasinya
    public GameObject[] decorPrefabs;

    // tipe cellnya, kosong atau cubiclenya atau wallnya
    private enum CellType { Empty, Cubicle, Wall, NotEmpty }
    // gridnya
    private CellType[,] grid;

    // Start is called before the first frame update
    void Start()
    {
        GenerateOffice();
    }

    private void GenerateOffice()
    {
        // kalau seednya udah pernah digunakan maka initseednya
        if (usedSeed) Random.InitState(seed);

        // bikin gridnya
        grid = new CellType[width, height];

        // generate wallnya
        for (int x = 0; x < width; x++) // sumbu x
        {
            for (int y = 0; y < height; y++) // sumbu y
            {
                if ((x == 0 && y == height / 2) || (x == width - 1 && y == height / 2))
                {
                    // kalau di tepi, maka wall
                    grid[x, y] = CellType.Wall;
                    SpawnWallPrefab(wallVerticalPrefab, x, y);
                }
                else if ((x == width / 2 && y == 0) || (x == width / 2 && y == height - 1))
                {
                    // kalau di tepi, maka wall
                    grid[x, y] = CellType.Wall;
                    SpawnWallPrefab(wallHorizontalPrefab, x, y);
                }
                else grid[x, y] = CellType.Empty; // kalau nggak ya dia empty
            }
        }

        // generate floornya
        for (int x = 0; x < width; x++) // sumbu x
        {
            for (int y = 0; y < height; y++) // sumbu y
            {
                if (x == width / 2 && y == height / 2)
                {
                    // kalau di tepi, maka wall
                    grid[x, y] = CellType.Empty;
                    SpawnPrefab(floorPrefab, x, y);
                }
            }
        }

        // generate cubiclenya
        for (int x = 2; x < width - 2; x++) // di tengah, bukan wallnya
        {
            for (int y = 2; y < height - 2; y++)
            {
                if (Random.value < 0.25f) // peluang kemunculan 25%
                {
                    grid[x, y] = CellType.Cubicle;
                    SpawnPrefab(cubiclePrefab, x, y);

                    // langsung add computernya, peluang 60%
                    if (Random.value < 0.6f) SpawnPrefab(computerPrefab, x, y);
                }
            }
        }

        // generate fotocopy machine, max 2
        int fotocopyMachineCount = 0;
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                // kalau empty dan peluangnya adalah 5% dan masih bisa spawn alias
                // kurang dari jumlahnya
                // munculnya cuma di pinggir
                if (grid[x, y] == CellType.Empty && Random.value < 0.05f &&
                    fotocopyMachineCount < 2 && (x == 1 || x == width - 1 ||
                    y == 1 || y == height - 1))
                {
                    grid[x, y] = CellType.NotEmpty;
                    SpawnPrefab(fotocopyMachinePrefab, x, y);
                    fotocopyMachineCount++;
                }
            }
        }

        // generate decorationsnya
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                // kalau empty dan peluangnya adalah 10%
                // cuma dipinggir dan kalau empty doang
                if (grid[x, y] == CellType.Empty && Random.value < 0.1f &&
                    (x == 1 || x == width - 1 || y == 1 || y == height - 1))
                {
                    // random deco yang mau di spawn
                    grid[x, y] = CellType.NotEmpty;
                    GameObject deco = decorPrefabs[Random.Range(0, decorPrefabs.Length)];
                    SpawnPrefab(deco, x, y);
                }
            }
        }
    }

    private void SpawnPrefab(GameObject prefab, int x, int y)
    {
        // posisi spawnnya
        Vector3 pos = new Vector3(x * cellSize, prefab.transform.position.y, y * cellSize);
        Instantiate(prefab, pos, Quaternion.identity, transform);
    }

    private void SpawnWallPrefab(GameObject prefab, int x, int y)
    {
        // posisi spawnnya
        Vector3 pos = new Vector3(x * cellSize, 0, y * cellSize);
        Instantiate(prefab, pos, prefab.transform.rotation, transform);
    }
}
