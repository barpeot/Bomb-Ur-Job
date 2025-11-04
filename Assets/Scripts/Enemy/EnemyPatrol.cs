using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour
{
    public Transform[] patrolPoints;
    public int targetPoint;
    public float moveSpeed = 2.0f;
    public float rotateSpeed = 360.0f;
    public float checkRadius = 0.5f;

    // kode nicho untuk chasing player
    // reference ke navmesh agentnya
    public NavMeshAgent agent;

    // reference ke player
    public Transform player;

    // reference kalau npc ngelihat si player
    [SerializeField] private bool isSeeing;

    // npc disuruh kemana?
    [SerializeField] private Vector3 targetPosition;

    // lagi ngubah rute ndak?
    private bool changeRoute = false;

    // id npc ini adalah?
    private int npcID;

    private enum State
    {
        Chase,
        Patrol
    }

    [SerializeField] private State currentState = State.Patrol;
    [SerializeField] private State lastState = State.Patrol;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        npcID = GetComponentInChildren<EnemyVision>().npcID;
    }

    private void OnEnable()
    {
        // event buat chase atau patrol nya
        EnemyVision.OnPlayerVisibilityChanged += HandleChasePatrol;
    }
    
    private void OnDisable() {
        EnemyVision.OnPlayerVisibilityChanged -= HandleChasePatrol;
    }
    // end of kode nicho

    // Start is called before the first frame update
    void Start()
    {
        targetPoint = 0;
        targetPosition = patrolPoints[targetPoint].position;
    }

    // Update is called once per frame
    void Update()
    {
        // set targetnya
        SetTarget();

        PatrolTowards();

        // if (transform.position == patrolPoints[targetPoint].position)
        // {
        //     AddTargetPoint();
        //     Debug.Log("ternyata nyentuh cuy");
        // }

        // instead of we manually check it, we use navmesh built in function 
        // to do it

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

    private void SetTarget()
    {
        if (currentState == lastState) return;

        lastState = currentState;

        if (currentState == State.Chase) targetPosition = player.position;
        else if (currentState == State.Patrol) targetPosition = patrolPoints[targetPoint].position;
    }

    void PatrolTowards()
    {
        // dapatkan arah
        Vector3 direction = (targetPosition - transform.position);

        //saat sampai tujuan, rotasi kembali kemudian update target
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, rotateSpeed);

        // transform.position = Vector3.MoveTowards(transform.position, patrolPoints[targetPoint].position, moveSpeed * Time.deltaTime);
        // kode nicho untuk chasing player
        // instead of we use a regular movetowards, we use it's agent to 
        // chase player
        agent.SetDestination(targetPosition);

        // kalau udah sampe, maka change route
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && !changeRoute) StartCoroutine(ChangeRouteCoroutine());
        // end of kode nicho
    }

    private IEnumerator ChangeRouteCoroutine()
    {
        // lagi changeroute
        changeRoute = true;

        AddTargetPoint();

        targetPosition = patrolPoints[targetPoint].position;

        yield return new WaitForSeconds(1);

        changeRoute = false;
    }

    // kode nicho untuk chasing player
    private void HandleChasePatrol(int id, bool seeing)
    {
        // cek id, kalau beda id nya maka bukan dirinya yang ngelihat
        // maka jangan chase
        if (npcID != id) return;

        // update disuruh gerak kemana
        if (seeing) currentState = State.Chase;
        else currentState = State.Patrol;
    }
    // end of kode nicho

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
