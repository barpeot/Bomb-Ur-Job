using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class OfficeGenerator : MonoBehaviour
{
    [Header("Non Interactables Count")]
    public int nonInteractablesCountSmall = 3;
    public int nonInteractablesCountMedium = 4;
    public int nonInteractablesCountLarge = 4;

    [Header("Patrol Point Count")]
    public int patrolPointCountSmall = 2;
    public int patrolPointCountMedium = 3;
    public int patrolPointCountLarge = 3;

    [Header("Interactables Count")]
    public int minInteractablesCountSmall = 5;
    public int maxInteractablesCountSmall = 6;
    public int minInteractablesCountMedium = 5;
    public int maxInteractablesCountMedium = 8;
    public int minInteractablesCountLarge = 6;
    public int maxInteractablesCountLarge = 8;

    [Header("Map Settings")]
    public float obstacleWallCountSmall = 2f;
    public float obstacleWallCountMedium = 4f;
    public float obstacleWallCountLarge = 4f;

    [Header("Map Size Settings")]
    public float smallGridCount = 16f;
    public float mediumGridCount = 18f;
    public float largeGridCount = 22f;
    public float smallScaleSize = 60f;
    public float mediumScaleSize = 80f;
    public float largeScaleSize = 100f;
    public enum MapSize
    {
        Small,
        Medium,
        Large
    }

    [Header("Grid Settings")]
    // ukuran dari mapnya (width sama height nya)
    public float widthGridCount = 10f;
    public float heightGridCount = 10f;
    // setiap cell ukurannya berapa
    public float cellSize = 2f;
    // seed nya
    public int seed = 0;
    // apakah seednya udah pernah digunakan
    public bool usedSeed = false;

    [Header("Floor Wall Obstacle Prefabs")]
    // floor plane
    public GameObject floorPlanePrefab;
    // wallnya
    public GameObject wallPrefab;
    // obstacle wallnya
    public GameObject obstacleWallPrefab;

    [Header("Prefabs Interactables")]
    // list prefabnya
    // komputernya
    public GameObject computerPrefab;
    // cubiclenya
    public GameObject cubiclePrefab;
    // office printernya
    public GameObject officePrinterPrefab;
    // water dispensernya
    public GameObject waterDispenserPrefab;
    // patrol point nya
    public GameObject patrolPointPrefab;

    [Header("Prefabs Non Interactables")]
    // list prefabnya
    // couchnya
    public GameObject[] nonInteractablesList;

    // tipe cellnya, kosong atau cubiclenya atau wallnya
    private enum CellType { Empty, Cubicle, Wall, NotEmpty }
    // gridnya
    private CellType[,] grid;

    // event apabila udah selesai generate officenya
    public static event Action OnFinishGenerateOffice;

    // event buat nambahin ke gamemanager patrol list si npc
    public static event Action<Transform> OnPatrolPointSpawned;

    // Start is called before the first frame update
    void Start()
    {
        GenerateOffice();

        // udah selesai generate office, maka invoke
        OnFinishGenerateOffice?.Invoke();
    }

    private void GenerateOffice()
    {
        // kalau seednya udah pernah digunakan maka initseednya
        if (usedSeed) UnityEngine.Random.InitState(seed);

        // reference ke obstacle countnya
        float obstacleCount = 0f;

        // reference ke jumlah interactablesnya
        float minInteractables = 0f;
        float maxInteractables = 0f;

        // reference ke jumlah patrol points nya
        float patrolPointCount = 0f;

        // reference ke jumlah non interactablesnya
        float nonInteractablesCount = 0f;

        // random dulu ukuran width dan heightnya
        MapSize randomSize = (MapSize)UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(MapSize)).Length);
        switch (randomSize)
        {
            case MapSize.Small: // kalau small 16
                widthGridCount = heightGridCount = smallGridCount;
                cellSize = smallScaleSize / smallGridCount;
                obstacleCount = obstacleWallCountSmall;
                minInteractables = minInteractablesCountSmall;
                maxInteractables = maxInteractablesCountSmall;
                patrolPointCount = patrolPointCountSmall;
                nonInteractablesCount = nonInteractablesCountSmall;
                break;
            case MapSize.Medium: // kalau medium 18
                widthGridCount = heightGridCount = mediumGridCount;
                cellSize = mediumScaleSize / mediumGridCount;
                obstacleCount = obstacleWallCountMedium;
                minInteractables = minInteractablesCountMedium;
                maxInteractables = maxInteractablesCountMedium;
                patrolPointCount = patrolPointCountMedium;
                nonInteractablesCount = nonInteractablesCountMedium;
                break;
            case MapSize.Large: // kalau large 22
                widthGridCount = heightGridCount = largeGridCount;
                cellSize = largeScaleSize / largeGridCount;
                obstacleCount = obstacleWallCountLarge;
                minInteractables = minInteractablesCountLarge;
                maxInteractables = maxInteractablesCountLarge;
                patrolPointCount = patrolPointCountLarge;
                nonInteractablesCount = nonInteractablesCountLarge;
                break;
        }

        // bikin gridnya
        grid = new CellType[(int)widthGridCount, (int)heightGridCount];

        // generate floornya berdasarkan grid countnya
        SpawnWall(floorPlanePrefab, 0, 0, widthGridCount, heightGridCount);

        // generate wallnya
        GenerateOuterWall();

        // set sebagai empty dulu semua grid di dalam wall
        for (int x = 1; x < widthGridCount - 1; x++)
        {
            for (int y = 1; y < heightGridCount - 1; y++)
            {
                grid[x, y] = CellType.Empty;
            }
        }

        // generate obstaclenya
        GenerateRandomObstacles(obstacleCount);

        // generate interactablesnya
        GenerateInteractables((int)minInteractables, (int)maxInteractables);

        // generate patrol point kosong nya
        GeneratePatrolPoints((int)patrolPointCount);

        // generate non interactablesnya
        GenerateNonInteractables((int)nonInteractablesCount);

        // generate spawn point nya
        GenerateSpawnPoint();
    }
    
    private void GenerateSpawnPoint()
    {
        // bikin pathway untuk npc (3) dan player (1) (2x2 saja)
        // player
        SpawnWall(floorPlanePrefab, (widthGridCount / 2) - 1, -2, 2, 2);
        // npc atas
        SpawnWall(floorPlanePrefab, (widthGridCount / 2) - 1, heightGridCount, 2, 2);
        // npc kiri
        SpawnWall(floorPlanePrefab, -2, (heightGridCount / 2) - 1, 2, 2);
        // npc kanan
        SpawnWall(floorPlanePrefab, heightGridCount, (heightGridCount / 2) - 1, 2, 2);

        // bikin spawn point untuk npc (3) dan player (1) (4x4 saja)
        // player
        SpawnSpawnPoint(floorPlanePrefab, (widthGridCount / 2) - 2, -6, 4, 4);
        // npc atas
        SpawnWall(floorPlanePrefab, (widthGridCount / 2) - 2, heightGridCount + 2, 4, 4);
        // npc kiri
        SpawnWall(floorPlanePrefab, -6, (heightGridCount / 2) - 2, 4, 4);
        // npc kanan
        SpawnWall(floorPlanePrefab, heightGridCount + 2, (heightGridCount / 2) - 2, 4, 4);

        // bikin all wallnya biar nutup si player
        // kiri spawn point
        SpawnWall(wallPrefab, (widthGridCount / 2) - 3, -6, 1, 4);
        // kanan spawn point
        SpawnWall(wallPrefab, (widthGridCount / 2) + 2, -6, 1, 4);
        // bawah spawn point
        SpawnWall(wallPrefab, (widthGridCount / 2) - 2, -7, 4, 1);
        // atas kiri spawn point
        SpawnWall(wallPrefab, (widthGridCount / 2) - 2, -2, 1, 2);
        // atas kanan spawn point
        SpawnWall(wallPrefab, (widthGridCount / 2) + 1, -2, 1, 2);
    }

    private void GenerateNonInteractables(int count)
    {
        int spawned = 0;
        while (spawned < count)
        {
            for (int x = 1; x < widthGridCount - 1; x++)
            {
                for (int y = 1; y < heightGridCount - 1; y++)
                {
                    // rules = x = 1-2
                    //         y = 1-2
                    if (grid[x, y] == CellType.Empty &&
                    (x == 1 || x == 2 || x == widthGridCount - 1 || x == widthGridCount - 2) &&
                    (y == 1 || y == 2 || y == heightGridCount - 1 || y == heightGridCount - 2) && 
                    (spawned < count))
                    {
                        // random dulu mau yang mana
                        int randomIndex = UnityEngine.Random.Range(0, nonInteractablesList.Length);

                        // 0 = couch
                        // 1 = cabinet
                        // 2 = vending machine

                        // peluang nya
                        bool peluangNon = UnityEngine.Random.value < 0.01f;
                        if (peluangNon)
                        {
                            // random rotasinya
                            int[] angles = { 0, 90, 180, 270 };
                            float rotY = angles[UnityEngine.Random.Range(0, angles.Length)];

                            SpawnPrefab(nonInteractablesList[randomIndex], x, y, Quaternion.Euler(0, rotY, 0), false);
                            spawned++;

                            // tag cell nya
                            grid[x, y] = CellType.NotEmpty;
                        }
                    }
                }
            }
        }
    }

    private void GeneratePatrolPoints(int count)
    {
        int spawned = 0;
        while (spawned < count)
        {
            for (int x = 3; x < widthGridCount - 3; x++)
            {
                for (int y = 3; y < heightGridCount - 3; y++)
                {
                    if (grid[x, y] == CellType.Empty && spawned < count)
                    {
                        bool peluangPatrolPoint = UnityEngine.Random.value < 0.01f;
                        if (peluangPatrolPoint)
                        {
                            SpawnPrefab(patrolPointPrefab, x, y, Quaternion.identity, true);
                            spawned++;
                            grid[x, y] = CellType.NotEmpty;
                        }
                    }
                    else continue;
                }
            }
        }
    }

    private void GenerateOuterWall()
    {
        float startingGridOfFirstWallHorizVerti = 0;
        float startingGridOfSecondWallHoriz = (widthGridCount / 2) + 1;
        float startingGridOfSecondWallVerti = (heightGridCount / 2) + 1;
        float totalGridOfWallHorizVerti = (widthGridCount / 2) - 1;
        // horiz bawah
        SpawnWall(wallPrefab, startingGridOfFirstWallHorizVerti, 0, totalGridOfWallHorizVerti, 1);
        SpawnWall(wallPrefab, startingGridOfSecondWallHoriz, 0, totalGridOfWallHorizVerti, 1);
        // horiz atas
        SpawnWall(wallPrefab, startingGridOfFirstWallHorizVerti, widthGridCount - 1, totalGridOfWallHorizVerti, 1);
        SpawnWall(wallPrefab, startingGridOfSecondWallHoriz, widthGridCount - 1, totalGridOfWallHorizVerti, 1);
        // verti kiri
        SpawnWall(wallPrefab, 0, startingGridOfFirstWallHorizVerti, 1, totalGridOfWallHorizVerti);
        SpawnWall(wallPrefab, 0, startingGridOfSecondWallVerti, 1, totalGridOfWallHorizVerti);
        // verti kanan
        SpawnWall(wallPrefab, heightGridCount - 1, startingGridOfFirstWallHorizVerti, 1, totalGridOfWallHorizVerti);
        SpawnWall(wallPrefab, heightGridCount - 1, startingGridOfSecondWallVerti, 1, totalGridOfWallHorizVerti);
    }

    private void GenerateRandomObstacles(float obstacleCount)
    {
        for (int i = 0; i < obstacleCount; i++)
        {
            // horiz apa verti
            bool isHorizontal = UnityEngine.Random.value > 0.5f;

            // tentukan panjangnya (dari 3 sampe 5)
            int obstacleLength = UnityEngine.Random.Range(3, 6);

            // random starting of x sama y nya
            // tapi dikurangi dengan panjangnya, biar nggak nabrak sama wall
            int minXY = 3;
            int maxX = (int)widthGridCount - 3 - obstacleLength;
            int maxY = (int)heightGridCount - 3 - obstacleLength;

            // random starting position of x nya
            float startingX = UnityEngine.Random.Range(minXY, maxX);
            float startingY = UnityEngine.Random.Range(minXY, maxY);

            // bikin obstaclenya sesuai horiz apa verti
            if (isHorizontal)
            {
                // spawn
                SpawnWall(wallPrefab, startingX, startingY, obstacleLength, 1);

                // tandain sebagai notempty dari starting ke ending
                for (int x = (int)startingX; x < startingX + obstacleLength; x++) grid[x, (int)startingY] = CellType.NotEmpty;
            }
            else
            {
                // spawn
                SpawnWall(wallPrefab, startingX, startingY, 1, obstacleLength);

                // tandain sebagai notempty dari starting ke ending
                for (int y = (int)startingY; y < startingY + obstacleLength; y++) grid[(int)startingX, y] = CellType.NotEmpty;
            }
        }
    }

    private void GenerateInteractables(int min, int max)
    {
        // random dulu berapa jumlahnya
        int count = UnityEngine.Random.Range(min, max + 1);
        Debug.Log($"count = {count}");

        // determined jumlah cubicle (computernya)
        int cubicleCount = (int)(0.5f * count);
        Debug.Log($"total cubicle = {cubicleCount}");
        // determined jumlah offife printer
        int printerCount = (int)Mathf.Ceil(0.25f * count);
        Debug.Log($"total printer = {printerCount}");
        // determined jumlah water dispenser nya
        int dispenserCount = count - cubicleCount - printerCount;
        Debug.Log($"total dispenser = {dispenserCount}");

        // spawn cubicle dulu
        // rules cubicle adalah, selisih antar barisnya harus >= 1
        int cubicleSpawned = 0;
        float prevRow = -999f;
        while (cubicleSpawned < cubicleCount)
        {
            // dapetin dulu rownya
            float row = UnityEngine.Random.Range(3, heightGridCount - 3);
            Debug.Log($"Mathf.Abs(row - prevRow) < 1 = {Mathf.Abs(row - prevRow) < 1}");
            if (Mathf.Abs(row - prevRow) < 1) continue; // kalau < 1 maka skip

            // random juga jumlah deretnya
            int deret = UnityEngine.Random.Range(1, 5);

            // dapetin juga starting column / x nya
            int startingX = (int)UnityEngine.Random.Range(3, widthGridCount - 2 - deret);
            Debug.Log($"startingX = {startingX}");

            // random rotasinya
            // 180 =  hadap z
            // 0 = hadap -z
            float rotY = UnityEngine.Random.value > 0.5f ? 0f : 180f;

            for (int i = 0; i < deret; i++)
            {
                // mulai spawn
                if (grid[(int)startingX, (int)row] == CellType.Empty && cubicleSpawned < cubicleCount)
                {
                    Vector3 padding = rotY == 180f ? new Vector3(0.08f, 0f, 0.25f) : new Vector3(-0.08f, 0f, -0.25f);
                    SpawnCubicleComputer("cubicle", (int)startingX, (int)row, rotY);
                    grid[(int)startingX, (int)row] = CellType.Cubicle;

                    // spawn juga computernya dengan peluang 60%
                    bool peluangComputer = UnityEngine.Random.value < 0.6;
                    if (peluangComputer)
                    {
                        // random rotasinya
                        float rotComY = rotY == 180f ? 270f : 90f;
                        SpawnCubicleComputer("computer", (int)startingX, (int)row, rotComY);
                    }

                    // tambahin cubiclespawnednya
                    cubicleSpawned++;
                    startingX++;
                }
                else continue;
            }
            prevRow = row;
        }

        // spawn printernya
        int printerSpawned = 0;
        while (printerSpawned < printerCount)
        {
            for (int x = 3; x < widthGridCount - 3; x++)
            {
                for (int y = 3; y < heightGridCount - 3; y++)
                {
                    if (grid[x, y] == CellType.Empty && printerSpawned < printerCount)
                    {
                        int[] angles = { 0, 90, 180, 270 };
                        float rotZ = angles[UnityEngine.Random.Range(0, angles.Length)];
                        bool peluangPrinter = UnityEngine.Random.value < 0.01f;
                        if (peluangPrinter)
                        {
                            SpawnPrefab(officePrinterPrefab, x, y, Quaternion.Euler(-90, 0, rotZ), true);
                            printerSpawned++;
                            grid[x, y] = CellType.NotEmpty;
                        }
                    }
                    else continue;
                }
            }
        }

        // spawn dispensernya
        int dispenserSpawned = 0;
        while (dispenserSpawned < dispenserCount)
        {
            for (int x = 3; x < widthGridCount - 3; x++)
            {
                for (int y = 3; y < heightGridCount - 3; y++)
                {
                    if (grid[x, y] == CellType.Empty && dispenserSpawned < dispenserCount)
                    {
                        int[] angles = { 0, 90, 180, 270 };
                        float rotZ = angles[UnityEngine.Random.Range(0, angles.Length)];
                        bool peluangDispenser = UnityEngine.Random.value < 0.01f;
                        if (peluangDispenser)
                        {
                            SpawnPrefab(waterDispenserPrefab, x, y, Quaternion.Euler(-90, 0, rotZ), true);
                            dispenserSpawned++;
                            grid[x, y] = CellType.NotEmpty;
                        }
                    }
                    else continue;
                }
            }
        }
    }

    private void SpawnPrefab(GameObject prefab, int x, int y, Quaternion rotation, bool isPatrolAdded)
    {
        // posisi spawnnya
        Vector3 pos = new Vector3(x * cellSize, prefab.transform.position.y, y * cellSize);
        GameObject newObj = Instantiate(prefab, pos, rotation, transform);

        if (isPatrolAdded) OnPatrolPointSpawned?.Invoke(newObj.transform);
    }

    private void SpawnCubicleComputer(string cubiOrCompu, int x, int y, float rotY)
    {
        // offset dasar cubicle
        float offsetZ = 0.28f;
        float offsetX = 0.1f;

        // offset dasar computer
        float offsetZCompu = 0.28f;
        float offsetXCompu = -0.1f;

        Vector3 padding = Vector3.zero;

        if (cubiOrCompu == "cubicle")
        {
            // Cubicle: posisinya di "tepi luar"
            if (Mathf.Approximately(rotY, 0f))
                padding = new Vector3(offsetX, 0f, offsetZ);   // menghadap +Z → cubicle di bawah (Z+)
            else if (Mathf.Approximately(rotY, 180f))
                padding = new Vector3(-offsetX, 0f, -offsetZ); // menghadap -Z → cubicle di atas (Z-)
        }
        else if (cubiOrCompu == "computer")
        {
            // Computer: posisinya di "dalam" cubicle (berlawanan arah)
            if (Mathf.Approximately(rotY, 90f))
                padding = new Vector3(-offsetXCompu, 0f, offsetZCompu); // di dalam cubicle yang hadap -Z
            else if (Mathf.Approximately(rotY, 270f))
                padding = new Vector3(offsetXCompu, 0f, -offsetZCompu);   // di dalam cubicle yang hadap +Z
        }

        SpawnPrefab(
            cubiOrCompu == "cubicle" ? cubiclePrefab : computerPrefab,
            x, y, Quaternion.Euler(0, rotY, 0), padding, cubiOrCompu == "computer" ? true : false
        );
    }

    private void SpawnPrefab(GameObject prefab, int x, int y, Quaternion rotation, Vector3 padding, bool isPatrolAdded)
    {
        Vector3 pos = new Vector3(
            (x * cellSize) + (padding.x * cellSize),
            prefab.transform.position.y,
            (y * cellSize) + (padding.z * cellSize)
        );

        GameObject newObj = Instantiate(prefab, pos, rotation, transform);

        if (isPatrolAdded) OnPatrolPointSpawned?.Invoke(newObj.transform);
    }

    private void SpawnWall(GameObject prefab, float startingX, float startingY, float totalWidth, float totalHeight)
    {
        float centerX = startingX + ((totalWidth - 1) / 2);
        float centerY = startingY + ((totalHeight - 1) / 2);
        float scaleX = totalWidth;
        float scaleY = totalHeight;

        Vector3 pos = new Vector3(centerX, prefab.transform.position.y, centerY) * cellSize;
        GameObject newObj = Instantiate(prefab, pos, prefab.transform.rotation, transform);
        newObj.transform.localScale = new Vector3(scaleX, newObj.transform.localScale.y, scaleY) * cellSize;
    }

    private void SpawnSpawnPoint(GameObject prefab, float startingX, float startingY, float totalWidth, float totalHeight)
    {
        float centerX = startingX + ((totalWidth - 1) / 2);
        float centerY = startingY + ((totalHeight - 1) / 2);
        float scaleX = totalWidth;
        float scaleY = totalHeight;

        Vector3 pos = new Vector3(centerX, prefab.transform.position.y, centerY) * cellSize;
        GameObject newObj = Instantiate(prefab, pos, prefab.transform.rotation, transform);
        newObj.transform.localScale = new Vector3(scaleX, newObj.transform.localScale.y, scaleY) * cellSize;

        GameManager.instance.player.transform.position = new Vector3(newObj.transform.position.x, 5, newObj.transform.position.z);
    }

    private void OnDrawGizmos()
    {
        if (grid == null) return;

        for (int x = 0; x < widthGridCount; x++)
        {
            for (int y = 0; y < heightGridCount; y++)
            {
                Vector3 pos = new Vector3(x * cellSize, 0, y * cellSize);

                switch (grid[x, y])
                {
                    case CellType.Empty:
                        Gizmos.color = Color.green;
                        break;
                    case CellType.Cubicle:
                        Gizmos.color = Color.blue;
                        break;
                    case CellType.Wall:
                        Gizmos.color = Color.red;
                        break;
                    case CellType.NotEmpty:
                        Gizmos.color = Color.red;
                        break;
                    default:
                        Gizmos.color = Color.white;
                        break;
                }

                Gizmos.DrawWireCube(pos, new Vector3(cellSize - 0.1f, 0.1f, cellSize - 0.1f));
            }
        }
    }
}
