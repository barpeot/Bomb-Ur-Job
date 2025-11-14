using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner instance;

    // reference ke npc yang mau di spawn
    public GameObject npcPrefab;

    // object pooling
    public Queue<GameObject> npcPool = new Queue<GameObject>();

    public static event Action OnAllEnemiesDead;


    private void Awake()
    {
        instance = this;
    }

    public int ActiveEnemies
    {
        get { return GameManager.instance.npcCount - npcPool.Count; }
    }

    private void OnEnable()
    {
        GameManager.OnFinishRebake += CreateNPCPool;
        EnemyController.OnEnemyDie += ReturnNPC;
    }

    private void OnDisable()
    {
        GameManager.OnFinishRebake -= CreateNPCPool;
        EnemyController.OnEnemyDie -= ReturnNPC;
    }

    // buat ngebikin poolnya apabila udah selesai generate office
    public void CreateNPCPool()
    {
        int jumlahNPC = GameManager.instance.npcCount;

        for (int count = 0; count < jumlahNPC; count++)
        {
            GameObject newObj = Instantiate(npcPrefab);
            newObj.SetActive(false);
            npcPool.Enqueue(newObj);
            Debug.Log($"npc count : {count} namanya adalah : {newObj.name}");
        }

        Debug.Log($"jumlah total pool = {npcPool.Count}");

        // setelah create pool, kita spawn semuanya
        StartCoroutine(SpawnPelanPelan(jumlahNPC));
    }

    private IEnumerator SpawnPelanPelan(int jumlah)
    {
        int jumlahSekarang = 0;
        while (jumlahSekarang < jumlah)
        {
            yield return StartCoroutine(SpawnNPC());
            jumlahSekarang++;
            yield return new WaitForSeconds(0.5f);
        }

        GameManager.instance.fillBar.fillAmount = 10f / 10f;
        UIController.instance.exposedPopUpUI.SetActive(false);
        GameManager.instance.lodingskrin.SetActive(false);
    }

    private IEnumerator SpawnNPC()
    {
        GameObject npc = GetNPC();
        // random positionnya dulu
        int randomPos = UnityEngine.Random.Range(0, 3);
        // 0 = kiri
        // 1 = atas
        // 2 = kanan

        // getcomponent dari si navmeshagent nya
        NavMeshAgent agentNPC = npc.GetComponent<NavMeshAgent>();

        yield return new WaitForSeconds(0.1f);

        switch (randomPos)
        {
            case 0:
                agentNPC.Warp(GameManager.instance.npcSpawnPointLeft);
                // npc.transform.position = GameManager.instance.npcSpawnPointLeft;
                break;
            case 1:
                agentNPC.Warp(GameManager.instance.npcSpawnPointTop);
                // npc.transform.position = GameManager.instance.npcSpawnPointTop;
                break;
            case 2:
                agentNPC.Warp(GameManager.instance.npcSpawnPointRight);
                // npc.transform.position = GameManager.instance.npcSpawnPointRight;
                break;
        }

        yield return new WaitForSeconds(0.1f);
    }

    // buat ngambil npc nya
    public GameObject GetNPC()
    {
        // di keluarin dari queue
        // lalu aktifkan
        // lalu balikin
        GameObject npcObj = npcPool.Dequeue();
        npcObj.SetActive(true);

        if (npcObj == null)
        {
            Debug.LogError("kosong cuy poolnya");
            return null;
        }
        return npcObj;
    }

    // buat ngembalikin npcnya ke queue
    public void ReturnNPC(GameObject npcObj)
    {
        // matikin dulu baru masukin lagi ke queue
        npcObj.SetActive(false);
        npcPool.Enqueue(npcObj);

        if (ActiveEnemies == 0)
        {
            OnAllEnemiesDead?.Invoke();
        }
    }
}
