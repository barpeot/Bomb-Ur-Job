using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public float speed = 6.0f;
    public bool canMove = true;
    public bool isMoving = true;
    
    [SerializeField] private float horizontalInput;
    [SerializeField] private float verticalInput;
    private float initialXrotation;

    private Animator anim;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        rb.freezeRotation = true;
        initialXrotation = transform.rotation.eulerAngles.x;
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
            anim.SetBool("isMoving", true);
            //melakukan rotasi player objek ke arah movement
            float targetAngle = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(initialXrotation, targetAngle, 0f);
            targetRotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                360 * Time.fixedDeltaTime
                );

            return targetRotation;
        }
        else
        {
            anim.SetBool("isMoving", false);
            return transform.rotation;
        }
       
    }

    public void setMove(bool move)
    {
        canMove = move;
    }

    void FixedUpdate()
    {
        Vector3 movement = getMovement();
        Quaternion targetRotation = getRotation(movement);

        //TODO movement dengan akselerasi dari starting speed ke maxspeed
        if (canMove)
        {
            rb.MovePosition(rb.position + movement * Time.fixedDeltaTime);
            rb.MoveRotation(targetRotation);
        }
    }

}
