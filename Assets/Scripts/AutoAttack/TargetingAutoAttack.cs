using UnityEngine;

public class TargetingAutoAttack : AutoAttack
{
    private GameObject target;

    public void SetTarget(GameObject target)
    {
        this.target = target;
    }

    public override void Attack(GameObject character)
    {
        if (target == null)
            return;


    }
}
