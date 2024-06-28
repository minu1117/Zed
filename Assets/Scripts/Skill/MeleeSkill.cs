using System.Collections;
using UnityEngine;

public class MeleeSkill : Skill
{
    public override void Use(GameObject character)
    {
        base.Use(character);
        StartCoroutine(CoMelee());
    }

    private IEnumerator CoMelee()
    {
        yield return waitduration;
        Release();
    }
}
