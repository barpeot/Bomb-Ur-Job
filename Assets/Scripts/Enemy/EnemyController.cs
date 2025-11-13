using System;
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
    public Animator anim;

    // [Header("State Machine")]
    // enemy state machine nya
    public EnemyStateMachine stateMachine { get; private set; }
    // state patrol
    public EnemyPatrolState patrolState;
    // chase state
    public EnemyChaseState chaseState;
    public EnemyCheckingState checkingState;
    public EnemyDieState dieState;

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

    // mati nggak
    public bool isDead = false;

    private void Awake()
    {
        player = GameManager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();
        stateMachine = new EnemyStateMachine();

        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        rb.isKinematic = true;
        rb.detectCollisions = false;

        patrolState = new EnemyPatrolState(this, stateMachine);
        chaseState = new EnemyChaseState(this, stateMachine);
        checkingState = new EnemyCheckingState(this, stateMachine);
        dieState = new EnemyDieState(this, stateMachine);

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

        //TODO: Tolong Kak Nicho bantu betulin handle integrasi animasi musuh dengan state machine
        // anim.SetBool("isMoving", true);

        stateMachine.Update();
    }

    private void OnEnable()
    {
        // subscribe ke event lagi ngelihat apa nggaknya
        EnemyVision.OnPlayerVisibilityChanged += HandleChasePatrol;

        // reenabled the navmesh and disabled the rb
        if (rb != null && !rb.isKinematic)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }

        if (agent != null) agent.enabled = true;
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
        stateMachine.ChangeState(dieState);

        // start coroutine buat matinya, balik ke object pool
        StartCoroutine(CoroutineDeath());
    }

    private IEnumerator CoroutineDeath()
    {
        yield return new WaitForSeconds(5f);
        OnEnemyDie?.Invoke(gameObject);
    }

    public void SpawnExplosionFX()
    {
        // pakai object pooling
        GameObject fx = ExplosionPool.instance.GetFromPool();
        fx.transform.position = transform.position + Vector3.up * 0.5f;
        fx.SetActive(true);

        // kalau udah masuk legi ke obj pool
        StartCoroutine(ReturnFXToPool(fx));
    }

    private IEnumerator ReturnFXToPool(GameObject fx)
    {
        // setelah 2 detik bakalan balik
        yield return new WaitForSeconds(2f);
        fx.SetActive(false);
        ExplosionPool.instance.ReturnToPool(fx);
    }
}
