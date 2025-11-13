using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDieState : EnemyState
{
    public EnemyDieState(EnemyController enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {

    }

    public override void Enter()
    {
        // set isdeadnya
        enemy.isDead = true;

        Debug.Log("masuk ke enter die state");
        Debug.Log($"currentstate = {stateMachine.currentState}");
        // Matikan NavMeshAgent biar gak bentrok sama physics
        enemy.agent.enabled = false;

        // Aktifkan physics
        enemy.rb.isKinematic = false;
        enemy.rb.detectCollisions = true;

        Debug.Log($"agent = {enemy.agent.enabled}");
        Debug.Log($"kinematik rb = {enemy.rb.isKinematic}");
        Debug.Log($"detectcollision = {enemy.rb.detectCollisions}");

        // bikin explosionnya
        enemy.rb.AddExplosionForce(300.0f, enemy.transform.position, 5.0f, 3.0F, ForceMode.Impulse);

        enemy.anim.SetBool("isMoving", false);
        enemy.anim.SetBool("isDying", true);

        // tampilkan efek explosionnya
        enemy.SpawnExplosionFX();

        Debug.Log($"Enemy {enemy.name} died!");
    }
}
