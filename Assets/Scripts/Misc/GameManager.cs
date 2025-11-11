using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // singleton
    public static GameManager instance;

    public bool isFullExposedBar = false;

    // nama gameplay scene nya
    private string gameplayScene = "scene nicho";

    // daftar npc yang ngelihat player
    private HashSet<int> npcSeeingPlayer = new HashSet<int>();

    // daftar patrol point nya npc
    public HashSet<Transform> npcPatrolList = new HashSet<Transform>();

    // reference ke player
    public GameObject player;

    // reference ke navmesh surface nya
    public NavMeshSurface navMeshSurface;

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

    private void OnEnable() {
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
        if (navMeshSurface != null) navMeshSurface.BuildNavMesh();
    }
    
    private void AddPatrolList(Transform patrolLocation)
    {
        npcPatrolList.Add(patrolLocation);
    }

    private void HandlePlayerVisibilityChanged(int npcID, bool seeing)
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
