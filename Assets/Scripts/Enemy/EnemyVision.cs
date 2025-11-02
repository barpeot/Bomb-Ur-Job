using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyVision : MonoBehaviour
{
    // reference ke player gameobject nya
    public GameObject player;

    // detection range nya
    public float detectionRange = 10f;

    // detectionanglenya
    public float detectionAngle = 45f;

    // reference ke debug text nya
    public TextMeshProUGUI isDetectedText;
    public TextMeshProUGUI isInRangeOfConeText;
    public TextMeshProUGUI isNotHiddenText;

    // reference exposed UI gameobject
    public GameObject exposedUI;

    // exposed bar increase speed
    [SerializeField] private float exposedBarIncreaseSpeed = 0.01f;
    [SerializeField] private float currentBarSpeed = 0f;

    // reference ke bar exposed nya
    public Image exposedBarImage;

    // tracker, setiap 2 detik, kecepatannya naik
    [SerializeField] private float secondTracker = 0f;
    [SerializeField] private float percepatanNambah = 0.05f;

    private void Start()
    {
        // langsung start coroutine increase exposed barnya
        StartCoroutine(IncreaseExposedBar());
    }

    // Update is called once per frame
    void Update()
    {
        EnemyDetection();
    }

    private void EnemyDetection()
    {
        // set semuanya false dulu
        GameManager.instance.isInRangeOfCone = false;
        GameManager.instance.isDetected = false;
        GameManager.instance.isNotHidden = false;

        // cek apakah jarak playernya tu kurang dari detecrange nya
        // kalau ya, maka player terdeteksi
        if (Vector3.Distance(player.transform.position, transform.position) < detectionRange)
        {
            // kedetect
            GameManager.instance.isDetected = true;

            // set tulisan debug ui nya
            isDetectedText.text = "player detected";
            isDetectedText.color = Color.red;
        }
        else
        {
            // kalau nggak kedetect
            // set tulisan debug ui nya
            isDetectedText.text = "player NOT detected";
            isDetectedText.color = Color.green;
        }

        // cek player hide atau nggak nya pakai raycast
        RaycastHit hit;
        if (Physics.Raycast(transform.position, (player.transform.position - transform.position), out hit, Mathf.Infinity))
        {
            // kalau pas nembak raycast tu yang kenak player, maka dia nggak hide
            if (hit.transform == player.transform)
            {
                GameManager.instance.isNotHidden = true;
                isNotHiddenText.text = "player exposed";
                isNotHiddenText.color = Color.red;
            }
            else
            {
                // nggak kenak player, maka hidden
                isNotHiddenText.text = "player hidden";
                isNotHiddenText.color = Color.green;
            }
        }
        else
        {
            // pas nembak nggak kenak apa apa, maka ya hidden juga
            // set lagi tulisannya
            isNotHiddenText.text = "player hidden";
            isNotHiddenText.color = Color.green;
        }

        // untuk deteksi apakah di dalam range conenya, dengan ngukur
        // angle antara npc dengan si playernya
        // sisi si player
        Vector3 side1 = player.transform.position - transform.position;
        // sisi si npc
        Vector3 side2 = transform.forward;
        // ngukur angle nya
        float angle = Vector3.SignedAngle(side1, side2, Vector3.up);

        // ukur anglenya
        // kalau diantara -45 hingga 45 maka in range
        if (angle < detectionAngle && angle > detectionAngle * -1)
        {
            GameManager.instance.isInRangeOfCone = true;
            isInRangeOfConeText.text = "player in detection angle";
            isInRangeOfConeText.color = Color.red;
        }
        else
        {
            isInRangeOfConeText.text = "player outside detection angle";
            isInRangeOfConeText.color = Color.green;
        }


        if (GameManager.instance.isDetected && GameManager.instance.isInRangeOfCone && GameManager.instance.isNotHidden)
        {
            // ketika exposed, maka barnya naik
            currentBarSpeed = exposedBarIncreaseSpeed;
            exposedUI.SetActive(true);
        }
        else
        {
            // kalau nggak maka nggak naik, dan reset semua
            exposedBarIncreaseSpeed = 0.05f;
            secondTracker = 0f;
            currentBarSpeed = 0f;
            exposedUI.SetActive(false);
        }
    }

    private IEnumerator IncreaseExposedBar()
    {
        // ambil dulu current level barnya
        float currentLevel = exposedBarImage.fillAmount;

        // tambahin setiap detik
        currentLevel += currentBarSpeed;
        currentLevel = Mathf.Clamp(currentLevel, 0, 1);
        exposedBarImage.fillAmount = currentLevel;
        // tambahin tracker secondnya
        secondTracker++;
        yield return new WaitForSeconds(1f);

        if (secondTracker == 2f)
        {
            // kalau udah 2 detik naik kecepatannya
            exposedBarIncreaseSpeed += percepatanNambah;
        }

        // kalau udah full, set full
        if (currentLevel == 1) GameManager.instance.isFullExposedBar = true;

        // infinite loop
        StartCoroutine(IncreaseExposedBar());
    }

    private void OnDrawGizmos()
    {
        if (player == null) return;

        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Vector3 leftBoundary = Quaternion.Euler(0, -detectionAngle, 0) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * detectionRange);

        Vector3 rightBoundary = Quaternion.Euler(0, detectionAngle, 0) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * detectionRange);

        if (GameManager.instance.isDetected && GameManager.instance.isNotHidden && GameManager.instance.isInRangeOfCone) 
        {
            Gizmos.color = Color.red;
        }
        else
        {
            Gizmos.color = Color.green;
        }
        Gizmos.DrawLine(transform.position, player.transform.position);
    }
}
