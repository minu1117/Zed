public class NormalEnemy : EnemyBase
{
    public override void Update()
    {
        base.Update();
        StateBehavior();
    }
}
