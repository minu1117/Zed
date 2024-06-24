using System.Collections;
using UnityEngine;

public class MeleeSkill : Skill
{
    public override void Use(GameObject character)
    {
        if (!IsUsed())
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
