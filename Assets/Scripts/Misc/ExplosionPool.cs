using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionPool : MonoBehaviour
{
    public static ExplosionPool instance;

    public GameObject explosionPrefab;

    private Queue<GameObject> pool = new Queue<GameObject>();

    private void Awake()
    {
        instance = this;
    }

    private IEnumerator Start()
    {
        yield return StartCoroutine(CreatePoolCoroutine());
    }

    private IEnumerator CreatePoolCoroutine()
    {
        // bikin 3 saja tapi pelan pelan biar CPU bisa napas
        for (int i = 0; i < 3; i++)
        {
            GameObject obj = Instantiate(explosionPrefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
            yield return new WaitForSeconds(0.1f);
        }
    }

    // buat ngambil
    public GameObject GetFromPool()
    {
        // kalau ada di return, kalau nggak ada maka di instantiate
        if (pool.Count > 0) return pool.Dequeue();
        else return Instantiate(explosionPrefab);
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
