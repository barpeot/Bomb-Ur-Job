using UnityEngine;
public class EnemyStateMachine
{
    // state sekarang
    public EnemyState currentState { get; private set; }

    // menginisialisasi statenya
    public void Initialize(EnemyState startingState)
    {
        currentState = startingState;
        startingState.Enter();
    }

    // ngubah statenya
    public void ChangeState(EnemyState newState)
    {
        currentState.Exit();
        currentState = newState;
        newState.Enter();
    }

    // logika updatenya
    public void Update()
    {
        currentState.LogicUpdate();
    }
}