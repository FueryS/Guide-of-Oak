/// <summary>
/// Abstract base for all enemy states.
/// Mirrors the PlayerBaseState pattern for consistency.
/// </summary>
public abstract class EnemyBaseState
{
    public abstract void EnterState(EnemyStateManager enemy);
    public abstract void UpdateState(EnemyStateManager enemy);
    public virtual void ExitState(EnemyStateManager enemy) { }
}