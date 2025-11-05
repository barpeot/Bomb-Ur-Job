using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCheckingState : EnemyState
{

    // constructor
    public EnemyCheckingState(EnemyController enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {
        
    }

    public override void Enter()
    {
        // start coroutine untuk ngecek apakah di sabotage
        enemy.StartCoroutine(CheckRoutine());
    }

    private IEnumerator CheckRoutine()
    {
        // Debug.Log($"{enemy.name} is checking the area");

        // set dulu timernya ke 0 detik
        float timer = 0f;

        while (timer < enemy.checkDuration) // kalau masih ada waktu ngecek
        {
            // maka check
            CheckNearbyInteractables();
            timer += Time.deltaTime;
            yield return null;
        }

        // kalau udah selesai ngecek balik ke patrol
        if (enemy != null) // kalau npcnya masih hidup
        {
            // maka balik ke patrol
            // Debug.Log($"{enemy.name} is done checking, back to patrol");

            stateMachine.ChangeState(enemy.patrolState);
        }
    }

    private void CheckNearbyInteractables()
    {
        Collider[] hits = Physics.OverlapSphere(enemy.transform.position, enemy.checkRadius);
        // Debug.Log($"hits = {hits[0].gameObject.name}");
        foreach (Collider hit in hits)
        {
            ObjectInteractable i = hit.GetComponent<ObjectInteractable>();
            // Debug.Log($"i != null = {i != null}");
            // Debug.Log($"i.isSabotaged = {i.isSabotaged}");
            // Debug.Log($"i = {i}");
            Debug.Log($"nama yang di hit = {hit.gameObject.name}");
            if (i != null && i.isSabotaged)
            {
                Debug.Log($"{enemy.name} found sabotaged object!");
                enemy.Die();
                return;
            }
        }
    }
}
