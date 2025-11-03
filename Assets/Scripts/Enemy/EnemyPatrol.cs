using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public Transform[] patrolPoints;
    public int targetPoint;
    public float moveSpeed = 2.0f;
    public float rotateSpeed = 360.0f;
    public float checkRadius = 0.5f;
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

        CheckNearbyInteractables();
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

    void CheckNearbyInteractables()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, checkRadius);
        foreach (Collider hit in hits)
        {
            ObjectInteractable i = hit.GetComponent<ObjectInteractable>();
            if (i != null && i.isSabotaged)
            {
                Debug.Log("Enemy found sabotaged object!");
                Die();
                return;
            }
        }
    }

    void Die()
    {
        Debug.Log("Enemy died!");
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}
