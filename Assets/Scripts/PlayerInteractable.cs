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
    public float interactRange = 2.0f;

    void Start()
    {
        interactSource = GetComponent<Transform>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // buat raycast ke depan untuk interaksi dengan objek
            Ray r = new Ray(interactSource.position, interactSource.forward);
            if(Physics.Raycast(r, out RaycastHit hitInfo, interactRange))
            {
                // saat terkena maka akan memanggil interact pada objek
                if(hitInfo.collider.gameObject.TryGetComponent(out IInteractable obj))
                {
                    obj.Interact();
                }
            }
        }
    }
}
