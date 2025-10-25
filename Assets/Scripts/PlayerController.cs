using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public float speed = 6.0f;
    [SerializeField] public float maxSpeed = 9.0f;
    
    [SerializeField] private float horizontalInput;
    [SerializeField] private float verticalInput;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private Vector3 getMovement()
    {
        //dapatkan axis horizontal dan vertikal untuk bergerak
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput) * speed;
        return movement;
    }

    private Quaternion getRotation(Vector3 movement)
    {
        if (movement != Vector3.zero)
        {
            //melakukan rotasi player objek ke arah movement
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            targetRotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                360 * Time.fixedDeltaTime
                );

            return targetRotation;
        }
        else
        {
            return Quaternion.identity;
        }
       
    }

    void FixedUpdate()
    {
        Vector3 movement = getMovement();
        Quaternion targetRotation = getRotation(movement);

        if (movement == Vector3.zero)
        {
            return;
        }

        //TODO movement dengan akselerasi dari starting speed ke maxspeed

        rb.MovePosition(rb.position + movement * Time.fixedDeltaTime);
        rb.MoveRotation(targetRotation);
    }

}
