using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ObjectInteractable : MonoBehaviour
{
    //TODO: Enum Objek apa saja yang akan diimplementasikan

    [SerializeField] public bool isSabotaged = false;
    [SerializeField] public float sabotageDuration = 5.0f;
    private Renderer rend;
    private void Start()
    {
        rend = GetComponent<Renderer>();
    }

    public void Interact()
    {
        //TODO: Saat proses melakukan sabotase akan muncul timer lingkaran

        if (!isSabotaged)
        {
            isSabotaged = !isSabotaged;
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
        if(isSabotaged)
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
        SetSabotage(true);
        Debug.Log("Item has been sabotaged!");
        
        yield return new WaitForSeconds(sabotageDuration);

        // setelah beberapa detik, item kembali seperti biasa
        SetSabotage(false);
        Debug.Log("Item has finished its sabotage!");
    }
}
