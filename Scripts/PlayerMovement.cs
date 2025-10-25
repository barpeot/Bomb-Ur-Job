using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playermovement : MonoBehaviour
{
    public CharacterController controller;
    public float MoveSpeed;
    private void Update()
    {
        Movement();
    }

    void Movement()
    {
        float inputAD;
        float inputWS;
        Vector3 inputWASD;
        inputAD = Input.GetAxis("Horizontal");
        inputWS = Input.GetAxis("Vertical");
        inputWASD = new Vector3(inputAD, 0, inputWS);
        transform.LookAt(transform.position + inputWASD);

        controller.Move(inputWASD * MoveSpeed * Time.deltaTime);
    }
}
