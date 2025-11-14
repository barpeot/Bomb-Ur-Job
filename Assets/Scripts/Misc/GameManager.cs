using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    public string mainMenuScene = "copyMainMenu";

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

    // audio
    public AudioSource BGMNormalSource;
    public AudioSource BGMChaseSource;
    public AudioSource SFXSource;
    public float fadeDuration = 0.5f;
    private Coroutine bgmCoroutine;
    public AudioClip explosionSFX;
    public AudioClip tickingSFX;

    public static event Action OnFinishRebake;

    public void PlayChaseBGM()
    {
        // kalau coroutine ada, maka di stop dulu
        if (bgmCoroutine != null) StopCoroutine(bgmCoroutine);
        StartCoroutine(CrossFadeBGM(BGMNormalSource, BGMChaseSource, 0.25f));
    }

    public void PlayNormalBGM()
    {
        // kalau coroutine ada, maka di stop dulu
        if (bgmCoroutine != null) StopCoroutine(bgmCoroutine);
        StartCoroutine(CrossFadeBGM(BGMChaseSource, BGMNormalSource, 0.25f));
    }

    private IEnumerator CrossFadeBGM(AudioSource fromSource, AudioSource toSource, float targetVolume)
    {
        // kalau clip yang dituju blom nyala, maka nyalain
        if (!toSource.isPlaying)
        {
            // play paused
            if (toSource.time > 0f) toSource.UnPause(); // pernah play
            else
            {
                // belum keplay
                toSource.volume = 0f;
                toSource.Play();
            }
        }
        else toSource.volume = 0f; // lagi play yaudah kasih ke 0 volumenya

        // detik sekarang
        float timeElapsed = 0f;
        // start volumenya
        float startVolFrom = fromSource.volume;
        float startVolTo = toSource.volume;

        // dikurangin pakai while
        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;

            // seberapa lembut penurunannya
            float smoothTime = timeElapsed / fadeDuration;

            fromSource.volume = Mathf.Lerp(startVolFrom, 0f, smoothTime);
            toSource.volume = Mathf.Lerp(startVolTo, targetVolume, smoothTime);

            yield return null;
        }

        // pastikan yang from volumenya 0 dan pause
        fromSource.volume = 0f;
        fromSource.Pause();
        toSource.volume = targetVolume;
    }

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

        // kalau fully exposed maka stop gamenya
        EnemyVision.OnFullyExposed += GameLose;

        EnemySpawner.OnAllEnemiesDead += GameWin;
    }

    private void OnDisable()
    {
        // nggak usah subscribe lagi kalau diancurin
        EnemyVision.OnPlayerVisibilityChanged -= HandlePlayerVisibilityChanged;

        OfficeGenerator.OnFinishGenerateOffice -= RebakeNavmeshSurface;

        OfficeGenerator.OnPatrolPointSpawned -= AddPatrolList;

        EnemyVision.OnFullyExposed -= GameLose;

        EnemySpawner.OnAllEnemiesDead -= GameWin;
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

        if (UIController.instance != null) UIController.instance.exposedPopUpUI.SetActive(anySeeingPlayer);
    }

    private void GameLose()
    {
        TextMeshProUGUI resultText = UIController.instance.youLoseUI.GetComponentInChildren<Image>().GetComponentInChildren<TextMeshProUGUI>();
        resultText.text = "You Lose!!!";
        UIController.instance.youLoseUI.SetActive(true);
        UIController.instance.exposedPopUpUI.SetActive(false);
        Time.timeScale = 0;
    }

    private void GameWin()
    {
        TextMeshProUGUI resultText = UIController.instance.youLoseUI.GetComponentInChildren<Image>().GetComponentInChildren<TextMeshProUGUI>();
        resultText.text = "You Win!!!";
        UIController.instance.youLoseUI.SetActive(true);
        UIController.instance.exposedPopUpUI.SetActive(false);
        Time.timeScale = 0;
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

    public void toMainMenu()
    {
        Destroy(GameManager.instance.gameObject);
        Destroy(UIController.instance.gameObject);
        Time.timeScale = 1;

        SceneManager.LoadScene(mainMenuScene);
    }
}
