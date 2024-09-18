public class NormalEnemy : EnemyBase
{
    public override void Update()
    {
        base.Update();
        Patrol();
        Chase();
        UseRandomSkill();
        CheackLoseTarget();
    }
}
