using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // singleton
    public static GameManager instance;

    public bool isFullExposedBar = false;

    // nama gameplay scene nya
    public string gameplayScene = "proceduralgeneratedscene";

    // daftar npc yang ngelihat player
    private HashSet<string> npcSeeingPlayer = new HashSet<string>();

    // daftar patrol point nya npc
    public List<Transform> npcPatrolList = new List<Transform>();

    // reference ke player
    public GameObject player;

    // reference ke navmesh surface nya
    public NavMeshSurface navMeshSurface;

    // jumlah npc yang mau di spawn
    public int npcCount = 0;

    // daftar ketiga spawn point npc
    public Vector3 npcSpawnPointLeft;
    public Vector3 npcSpawnPointTop;
    public Vector3 npcSpawnPointRight;

    public Image fillBar;
    public GameObject lodingskrin;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        // subscribe ke enemyvision. kaya lagi ngeset kuping buat
        // ndengerin npc. kalau kedengeran, maka ngapain
        // maka akan munculin pop up exposed
        EnemyVision.OnPlayerVisibilityChanged += HandlePlayerVisibilityChanged;

        // kalau udah selesai generate office, maka bake ulang navmeshsurfacenya
        OfficeGenerator.OnFinishGenerateOffice += RebakeNavmeshSurface;

        // kalau ada patrol point yang di add, maka tambahin kesini
        OfficeGenerator.OnPatrolPointSpawned += AddPatrolList;
    }

    private void OnDisable()
    {
        // nggak usah subscribe lagi kalau diancurin
        EnemyVision.OnPlayerVisibilityChanged -= HandlePlayerVisibilityChanged;

        OfficeGenerator.OnFinishGenerateOffice -= RebakeNavmeshSurface;

        OfficeGenerator.OnPatrolPointSpawned -= AddPatrolList;
    }

    private void RebakeNavmeshSurface()
    {
        if (navMeshSurface != null) StartCoroutine(StartRebakeCoroutine());
    }

    private IEnumerator StartRebakeCoroutine()
    {
        yield return StartCoroutine(RebakeCoroutine());
    }

    private IEnumerator RebakeCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        navMeshSurface.BuildNavMesh();
        yield return new WaitForSeconds(0.1f);
        fillBar.fillAmount = 9 / 9;
        lodingskrin.SetActive(false);
    }

    private void AddPatrolList(Transform patrolLocation)
    {
        npcPatrolList.Add(patrolLocation);
    }

    private void HandlePlayerVisibilityChanged(string npcID, bool seeing)
    {
        if (seeing) npcSeeingPlayer.Add(npcID);
        else npcSeeingPlayer.Remove(npcID);

        // kita munculin atau sembunyiin sesuai dengan ada tidaknya npc
        // yang ngelihat player
        UpdateExposedPopUp();
    }

    private void UpdateExposedPopUp()
    {
        // kita ambil dulu berapa banyak npc yang ngelihat plauyer
        bool anySeeingPlayer = npcSeeingPlayer.Count > 0;

        if (UIController.instance != null)
            UIController.instance.exposedPopUpUI.SetActive(anySeeingPlayer);

        if (isFullExposedBar)
        {
            UIController.instance.youLoseUI.SetActive(true);
            UIController.instance.exposedPopUpUI.SetActive(false);
            Time.timeScale = 0;
        }
    }

    public void Restart()
    {
        // balik lagi bisa dimain kan
        Time.timeScale = 1;
        SceneManager.LoadScene(gameplayScene);
        isFullExposedBar = false;
        // all ui inactive
        UIController.instance.youLoseUI.SetActive(false);
        UIController.instance.exposedPopUpUI.SetActive(false);
    }
}
