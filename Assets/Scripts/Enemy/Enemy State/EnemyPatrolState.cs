using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrolState : EnemyState
{
    // target posisi patrol saat ini
    private int currentPoint = 0;
    // lagi nunggu nggak?
    private bool isWaiting;

    // constructor
    public EnemyPatrolState(EnemyController enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {
        
    }

    public override void Enter()
    {
        // ke patrol point selanjutnya
        GoToNextPoint();
    }

    public override void LogicUpdate()
    {
        // kalau udah selesai menuju ke patrol point, maka tunggu dan ganti tujuan
        if (!enemy.agent.pathPending && enemy.agent.remainingDistance <= enemy.agent.stoppingDistance + 0.1f && !isWaiting)
        {
            enemy.StartCoroutine(WaitAndMoveNext());
        }
    }

    private void GoToNextPoint()
    {
        // kalau patrolpoints nya nggak ada, maka skip
        if (enemy.patrolPoints.Length == 0) return;

        // random dulu tujuan nya
        currentPoint = Random.Range(0, enemy.patrolPoints.Length);

        // set destination selanjutnya
        enemy.agent.SetDestination(enemy.patrolPoints[currentPoint].position);
    }

    private IEnumerator WaitAndMoveNext()
    {
        // lagi nunggu
        isWaiting = true;

        // ketika nunggu pindah ke state checking
        stateMachine.ChangeState(enemy.checkingState);

        yield return new WaitForSeconds(enemy.waitTimeAtPoint);

        // tambahin currentpointnya
        // currentPoint = (currentPoint + 1) % enemy.patrolPoints.Length;

        // langsung set tujuan kesana
        GoToNextPoint();

        // udah nggak nunggu lagi
        isWaiting = false;
    }

    public override void Exit()
    {
        
    }
}
