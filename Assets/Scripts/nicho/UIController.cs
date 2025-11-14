using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    // singleton
    public static UIController instance;

    // reference ke ui you lose dan ui exposed pop up
    public GameObject youLoseUI, exposedPopUpUI, npcCountUI, enemySpawner;

    private TextMeshProUGUI npcCountText;
    private EnemySpawner enmy;

    // reference ke exposed barnya
    public Image exposedBarImage, bgExposedBar;

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

    // Start is called before the first frame update
    void Start()
    {
        // pastikan semua UI inactive
        youLoseUI.SetActive(false);
        exposedPopUpUI.SetActive(false);
        npcCountText = npcCountUI.GetComponent<TextMeshProUGUI>();
        enmy = enemySpawner.GetComponent<EnemySpawner>();
    }

    private void Update()
    {
        UpdateEnemyCount();
    }

    private void UpdateEnemyCount()
    {
        int npcCountMax = GameManager.instance.npcCount;
        int npcCountActive = EnemySpawner.instance.ActiveEnemies;

        npcCountText.text = (npcCountMax - npcCountActive).ToString() + " / " + npcCountMax.ToString();
    }
}
