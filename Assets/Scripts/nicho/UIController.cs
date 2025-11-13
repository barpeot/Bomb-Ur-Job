using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    // singleton
    public static UIController instance;

    // reference ke ui you lose dan ui exposed pop up
    public GameObject youLoseUI, exposedPopUpUI;
    public GameManager gamemanager;

    // reference ke exposed barnya
    public Image exposedBarImage;

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
    }
}
