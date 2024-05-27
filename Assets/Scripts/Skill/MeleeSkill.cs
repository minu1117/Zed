using System.Collections;
using UnityEngine;

public class MeleeSkill : Skill
{
    public override void Use(GameObject character)
    {
        if (!isComplated || isCoolTime)
            return;

        base.Use(character);
        StartCoroutine(CoMelee());
    }

    private IEnumerator CoMelee()
    {
        yield return waitduration;
        ReleaseFunc();
    }
}
