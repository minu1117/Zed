public class NormalEnemy : EnemyBase
{
    public void Update()
    {
        Patrol();
        Chase();
        UseRandomSkill();
        CheackLoseTarget();
    }
}
