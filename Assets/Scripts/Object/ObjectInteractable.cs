using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class ObjectInteractable : MonoBehaviour, IInteractable
{
    //TODO: Enum Objek apa saja yang akan diimplementasikan

    public bool isSabotaged = false;
    public float timeToSabotage = 2.0f;
    public float sabotageDuration = 2.0f;

    public GameObject player;
    public PlayerController playerController;
    [SerializeField] private Image timerUI;
    private Renderer rend;

    private void Start()
    {
        player = GameManager.instance.player;
        rend = GetComponent<Renderer>();
        // timerUI = transform.Find("TimerUICanvas/TimerUI").GetComponent<Image>();
        playerController = player.GetComponent<PlayerController>();
    }

    public void Interact()
    {
        //TODO: Saat proses melakukan sabotase akan muncul timer lingkaran

        if (!isSabotaged)
        {
            // saat sabotase akan berubah warna selama beberapa detik
            StartCoroutine(SabotageRoutine());
        }
        else
        {
            Debug.Log("Object already sabotaged");
        }

    }

    private void SetSabotage(bool state)
    {
        //TODO: Saat dilakukan sabotase objeknya akan berubah sedikit?

        isSabotaged = state;
        if (isSabotaged)
        {
            rend.material.color = Color.red;
        }
        else
        {
            rend.material.color = Color.white;
        }
    }

    private IEnumerator SabotageRoutine()
    {
        //TODO: Setiap objek butuh waktu sabotase yang berbeda-beda, setiap objek durasi sabotasenya berbeda-beda juga

        // sabotase item selama beberapa detik
        yield return StartCoroutine(FillRadial(timeToSabotage));

        // setelah beberapa detik, item kembali seperti biasa
        yield return StartCoroutine(CountdownTimer(sabotageDuration));
    }

    private IEnumerator FillRadial(float duration)
    {

        float clock = 0f;
        timerUI.fillAmount = 0;

        while (clock < duration && timerUI.fillAmount != 1)
        {
            clock += Time.deltaTime;
            float progress = clock / duration;
            timerUI.fillAmount = Mathf.Clamp01(progress);
            yield return null;
        }

        timerUI.fillAmount = 1f;
        Debug.Log("Item has been sabotaged!");
        SetSabotage(true);
    }

    private IEnumerator CountdownTimer(float duration)
    {
        // play audio ticking nya
        GameManager.instance.SFXSource.clip = GameManager.instance.tickingSFX;
        GameManager.instance.SFXSource.Play();

        float clock = 0f;
        timerUI.fillAmount = 1f;

        while (clock < duration && timerUI.fillAmount != 0)
        {
            clock += Time.deltaTime;
            float remaining = 1f - (clock / duration);
            timerUI.fillAmount = Mathf.Clamp01(remaining);
            yield return null;
        }

        timerUI.fillAmount = 0f;
        Debug.Log("Item has stopped being sabotaged!");
        SetSabotage(false);

        // stop audio ticking nya
        GameManager.instance.SFXSource.Stop();
    }
}
