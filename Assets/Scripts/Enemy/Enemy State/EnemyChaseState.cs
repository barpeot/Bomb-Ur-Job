using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseState : EnemyState
{
    // constructor
    public EnemyChaseState(EnemyController enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {

    }

    public override void Enter()
    {
        // nggak lagi diem
        enemy.agent.isStopped = false;

        // animasi
        enemy.anim.SetBool("isMoving", true);
    }

    public override void LogicUpdate()
    {
        // kalau playernya nggak ada atau npc udah mati, maka skip
        if (enemy.player == null || enemy.isDead) return;

        // kejar player
        enemy.agent.SetDestination(enemy.player.position);
    }

    public override void Exit()
    {
        // reset path nya, biar dia nggak nyangkut pas balik lagi ke patrol
        enemy.agent.ResetPath();
    }
}
