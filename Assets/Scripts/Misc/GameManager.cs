using System;
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
    public HashSet<string> npcSeeingPlayer = new HashSet<string>();

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

    public bool debuganyseeingplayer = false;
    public int debugnpcseeingplayercount = 0;

    public static event Action OnFinishRebake;

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

        // rebake juga lightingnya
        StartCoroutine(RefreshLighting());

        fillBar.fillAmount = 9f / 10f;

        OnFinishRebake?.Invoke();
    }

    private IEnumerator RefreshLighting()
    {
        // Paksa Unity update lighting data runtime (untuk Realtime GI)
        yield return new WaitForSeconds(0.1f);
        DynamicGI.UpdateEnvironment();
        Debug.Log("Realtime lighting updated!");
        yield return new WaitForSeconds(0.1f);
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
        fillBar.fillAmount = 9f / 10f;
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
        debuganyseeingplayer = anySeeingPlayer;
        debugnpcseeingplayercount = npcSeeingPlayer.Count;

        if (UIController.instance != null && !isFullExposedBar)
            UIController.instance.exposedPopUpUI.SetActive(anySeeingPlayer);
        else
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
        UIController.instance.exposedBarImage.fillAmount = 0f;
        // all ui inactive
        UIController.instance.youLoseUI.SetActive(false);
        UIController.instance.exposedPopUpUI.SetActive(false);

        // reset semuanya
        npcPatrolList.Clear();
        npcSeeingPlayer.Clear();
        lodingskrin.SetActive(true);
        fillBar.fillAmount = 0f;
    }
}
