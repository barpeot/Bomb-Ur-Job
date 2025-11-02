using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // singleton
    public static GameManager instance;

    // apakah player dalam range conenya, apakah player terdeteksi dalam cone
    // apakah player tidak bersembunyi
    public bool isInRangeOfCone, isDetected, isNotHidden;

    public bool isFullExposedBar = false;

    // nama gameplay scene nya
    private string gameplayScene = "scene nicho";

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

    // Update is called once per frame
    void Update()
    {
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
