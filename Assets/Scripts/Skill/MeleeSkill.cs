using System.Collections;
using UnityEngine;

public class MeleeSkill : Skill
{
    public override void Use(GameObject charactor)
    {
        StartCoroutine(CoMelee());
    }

    private IEnumerator CoMelee()
    {
        yield return new WaitForSeconds(data.useDelay + data.duration);
        pool.Release(this);
    }
}
