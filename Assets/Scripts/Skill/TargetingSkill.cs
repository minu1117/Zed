using UnityEngine;

public class TargetingSkill : Skill, ITargetable
{
    protected GameObject target;

    public void SetTarget(GameObject target)
    {
        this.target = target;
    }

    public override void Use(GameObject character)
    {
        if (!IsUsed())
            return;

        base.Use(character);
    }
}
