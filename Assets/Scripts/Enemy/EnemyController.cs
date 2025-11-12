using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("References")]
    // rute patroli
    public Transform[] patrolPoints;
    // reference ke player
    public Transform player;
    // navmesh agent nya
    public NavMeshAgent agent;
    public Rigidbody rb;
    public GameObject explosionAsset;

    // [Header("State Machine")]
    // enemy state machine nya
    public EnemyStateMachine stateMachine { get; private set; }
    // state patrol
    public EnemyPatrolState patrolState;
    // chase state
    public EnemyChaseState chaseState;
    public EnemyCheckingState checkingState;

    [Header("Settings")]
    // waktu diem di tempat
    public float waitTimeAtPoint = 1f;
    // id npc nya
    public int npcID;

    [Header("Check Settings")]
    // dari kak akbar
    public float checkRadius = 0.5f;
    public float checkDuration = 2f;

    private void Awake() {
        agent = GetComponent<NavMeshAgent>();
        stateMachine = new EnemyStateMachine();
        npcID = GetComponentInChildren<EnemyVision>().npcID;
        rb = GetComponent<Rigidbody>();

        patrolState = new EnemyPatrolState(this, stateMachine);
        chaseState = new EnemyChaseState(this, stateMachine);
        checkingState = new EnemyCheckingState(this, stateMachine);

        // set toleransi stopnya pathfinding
        agent.stoppingDistance = 0.2f;

        // ambil patrol point dari  gamemanager
        // patrolPoints = GameManager.instance.npcPatrolList;
    }

    // Start is called before the first frame update
    void Start()
    {
        // di initialize dulu state nya ke patrol
        stateMachine.Initialize(patrolState);
    }

    // Update is called once per frame
    void Update()
    {
        // ngupdate
        stateMachine.Update();
    }

    private void OnEnable()
    {
        // subscribe ke event lagi ngelihat apa nggaknya
        EnemyVision.OnPlayerVisibilityChanged += HandleChasePatrol;
    }

    private void OnDisable()
    {
        // unsubscribe
        EnemyVision.OnPlayerVisibilityChanged -= HandleChasePatrol;
    }

    private void HandleChasePatrol(int id, bool seeing)
    {
        // kalau bukan dirinya, maka jangan kejar
        if (npcID != id) return;

        // kalau ngelihat maka ubah statenya ke chase
        if (seeing) stateMachine.ChangeState(chaseState);
        else stateMachine.ChangeState(patrolState);
    }
    
    public void Die()
    {
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, 5.0f);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
                rb.AddExplosionForce(150.0f, explosionPos, 5.0f, 3.0F);
        }
        Debug.Log($"Enemy {name} died!");
    }
}
