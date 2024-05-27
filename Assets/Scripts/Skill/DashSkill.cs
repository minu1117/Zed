using System.Collections;
using UnityEngine;

public class DashSkill : Skill
{
    public override void Use(GameObject character)
    {
        if (!isComplated || isCoolTime)
            return;

        base.Use(character);
        StartCoroutine(CoDash(character, Raycast.GetMousePointVec()));
    }

    private IEnumerator CoDash(GameObject character, Vector3 point)
    {
        var rb = character.GetComponent<Rigidbody>();
        Zed zed = GetStopZed(character);

        rb.velocity = Vector3.zero;
        point.y = character.transform.position.y;

        Vector3 LookAtDirection = (point == Vector3.zero) ? transform.forward : point;
        Vector3 dashDirection = (point - transform.position).normalized;
        character.transform.LookAt(LookAtDirection);

        yield return waitUseDelay;

        rb.velocity = dashDirection * data.speed;

        yield return waitduration;
        
        rb.velocity = Vector3.zero;

        yield return waitimmobilityTime;

        OnComplate();
        OnMoveZed(zed);
    }
}
