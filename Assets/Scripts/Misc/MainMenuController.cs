using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public GameObject tutorialMenu;
    public GameObject titleMenu;
    public GameObject creditsMenu;

    public string gameScene;

    public void PlayGame()
    {
        SceneManager.LoadScene(gameScene);
    }
    
    public void OpenTutorial()
    {
        tutorialMenu.SetActive(true);
        titleMenu.SetActive(false);
    }

    public void CloseTutorial()
    {
        tutorialMenu.SetActive(false);
        titleMenu.SetActive(true);
    }

    public void OpenCredits()
    {
        creditsMenu.SetActive(true);
        titleMenu.SetActive(false);
    }

    public void CloseCredits()
    {
        creditsMenu.SetActive(false);
        titleMenu.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
