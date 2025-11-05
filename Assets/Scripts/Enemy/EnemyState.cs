public abstract class EnemyState
{
    // reference ke enemycontroller
    protected EnemyController enemy;
    // reference ke enemy state  machine nya
    protected EnemyStateMachine stateMachine;

    // constructor
    public EnemyState(EnemyController enemy, EnemyStateMachine stateMachine)
    {
        this.enemy = enemy;
        this.stateMachine = stateMachine;
    }

    // blueprint buat setiap methodnya
    public virtual void Enter() { }
    public virtual void LogicUpdate() { }
    public virtual void Exit() { }
}