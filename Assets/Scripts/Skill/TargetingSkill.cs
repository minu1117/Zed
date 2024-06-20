using UnityEngine;

public class TargetingSkill : Skill
{
    public override void Use(GameObject character)
    {
        if (!isComplated || isCoolTime)
            return;

        base.Use(character);
    }
}
