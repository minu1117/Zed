public class NormalEnemy : EnemyBase
{
    public override void Update()
    {
        base.Update();
        //Patrol();
        //Chase();
        //UseRandomSkill();
        //CheackLoseTarget();
        StateBehavior();
    }

    private void StateBehavior()
    {
        switch (state)
        {
            case State.Patrol:
                Patrol();
                break;
            case State.Chase:
                Chase();
                break;
            case State.Attack:
                UseRandomSkill();
                break;
        }
    }
}
