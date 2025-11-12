using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IInteractable
{
    public void Interact();
}

public class PlayerInteractable : MonoBehaviour
{
    public Transform interactSource;
    public float interactRange = 2.5f;

    private IInteractable currentInteractable = null;
    public GameObject interactUI;

    void Start()
    {
        interactSource = GetComponent<Transform>();
        //interactUI dinonaktifkan
    }

    void Update()
    {        
        // buat raycast ke depan untuk interaksi dengan objek
        Ray r = new Ray(interactSource.position, interactSource.forward);
        if (Physics.Raycast(r, out RaycastHit hitInfo, interactRange))
        {
            // saat terkena maka akan memanggil interact pada objek
            if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable obj))
            {
                currentInteractable = obj;

                // Tampilkan tooltip press E
                interactUI.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    // jika press E, interact
                    obj.Interact();
                }
            }
        }
        else
        {
            // jika tidak ada, maka press E dimatikan
            currentInteractable = null;
            interactUI.SetActive(false);
        }
    }

}
