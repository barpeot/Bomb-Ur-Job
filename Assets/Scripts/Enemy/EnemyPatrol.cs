using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public Transform[] patrolPoints;
    public int targetPoint;
    public float moveSpeed = 2.0f;
    public float rotateSpeed = 360.0f;
    // Start is called before the first frame update
    void Start()
    {
        targetPoint = 0;   
    }

    // Update is called once per frame
    void Update()
    {
        PatrolTowards();
       
        if (transform.position == patrolPoints[targetPoint].position)
        {
            AddTargetPoint();
        }

    }

    void AddTargetPoint()
    {
        targetPoint++;
        if (targetPoint >= patrolPoints.Length)
        {
            targetPoint = 0;
        }
    }

    void PatrolTowards()
    {
        // dapatkan target
        Transform target = patrolPoints[targetPoint];

        // dapatkan arah
        Vector3 direction = (target.position - transform.position);

        //saat sampai tujuan, rotasi kembali kemudian update target
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, rotateSpeed);

        transform.position = Vector3.MoveTowards(transform.position, patrolPoints[targetPoint].position, moveSpeed * Time.deltaTime);
    }
}
