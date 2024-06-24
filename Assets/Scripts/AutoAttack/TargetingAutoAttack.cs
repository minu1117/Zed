using UnityEngine;

public class TargetingAutoAttack : AutoAttack, ITargetable
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

        Vector3 point = target.transform.position;
        point.y = character.transform.position.y;
        character.transform.LookAt(point);
    }
}
