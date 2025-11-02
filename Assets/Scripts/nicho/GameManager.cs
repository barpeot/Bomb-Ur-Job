using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // singleton
    public static GameManager instance;

    // apakah player dalam range conenya, apakah player terdeteksi dalam cone
    // apakah player tidak bersembunyi
    public bool isInRangeOfCone, isDetected, isNotHidden;

    // reference ke ui you lose dan ui exposed pop up
    public GameObject youLoseUI, exposedPopUpUI;

    public bool isFullExposedBar = false;

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

    private void Start()
    {
        // pastikan semua UI inactive
        youLoseUI.SetActive(false);
        exposedPopUpUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isFullExposedBar)
        {
            Time.timeScale = 0;
            youLoseUI.SetActive(true);
            exposedPopUpUI.SetActive(false);
        }
    }
}
