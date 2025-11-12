using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public string npcID;

    [Header("Check Settings")]
    // dari kak akbar
    public float checkRadius = 0.5f;
    public float checkDuration = 2f;

    // event ketika npc mati
    public static event Action<GameObject> OnEnemyDie;

    private void Awake() {
        player = GameManager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();
        stateMachine = new EnemyStateMachine();

        patrolState = new EnemyPatrolState(this, stateMachine);
        chaseState = new EnemyChaseState(this, stateMachine);
        checkingState = new EnemyCheckingState(this, stateMachine);

        // set toleransi stopnya pathfinding
        agent.stoppingDistance = 0.2f;

        // ambil patrol point dari  gamemanager
        patrolPoints = GameManager.instance.npcPatrolList.ToArray();
    }

    // Start is called before the first frame update
    void Start()
    {
        npcID = GetComponentInChildren<EnemyVision>().npcID;
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

    private void HandleChasePatrol(string id, bool seeing)
    {
        // kalau bukan dirinya, maka jangan kejar
        if (npcID != id) return;

        // kalau ngelihat maka ubah statenya ke chase
        if (seeing) stateMachine.ChangeState(chaseState);
        else stateMachine.ChangeState(patrolState);
    }
    
    public void Die()
    {
        Instantiate(explosionAsset, transform.position, Quaternion.identity, null);
        Debug.Log($"Enemy {name} died!");
        // Destroy(gameObject);
        OnEnemyDie?.Invoke(gameObject);
    }
}