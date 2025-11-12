using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    // reference ke loading screen objectnya
    public GameObject loadingScreen;
    // reference ke barnya
    public Image loadingBar;

    public void LoadScene()
    {
        StartCoroutine(LoadSceneAsync("proceduralgeneratedscene"));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        // nyalain gameobjectnya
        loadingScreen.SetActive(true);

        // ketika lom selesai maka jangan lanjut
        while (!operation.isDone)
        {
            // progress value nya
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);

            // kasih di loadingbarnya
            loadingBar.fillAmount = progressValue;

            yield return null;
        }
    }
}
