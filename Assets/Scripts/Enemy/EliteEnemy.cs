public class EliteEnemy : EnemyBase
{
    public enum AttackState
    {
        AutoAttack,
        Skill,
    }

    private AttackState attackState;

    public override void Update()
    {
        base.Update();

    }
}
