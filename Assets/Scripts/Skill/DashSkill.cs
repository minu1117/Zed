using System.Collections;
using UnityEngine;

public class DashSkill : Skill
{
    public Vector3 movePoint;

    public override void Use(GameObject character)
    {
        if (!isComplated || isCoolTime)
            return;

        base.Use(character);

        if (movePoint == null || movePoint == Vector3.zero)
            movePoint = Raycast.GetMousePointVec();
        
        StartCoroutine(CoDash(character, movePoint));
    }

    public void SetPoint(Vector3 point)
    {
        movePoint = point;
    }

    private IEnumerator CoDash(GameObject obj, Vector3 point)
    {
        var rb = obj.GetComponent<Rigidbody>();
        CharacterMoveController character = GetStopCharacter(obj);

        rb.velocity = Vector3.zero;
        point.y = obj.transform.position.y;

        Vector3 LookAtDirection = (point == Vector3.zero) ? transform.forward : point;
        Vector3 dashDirection = (point - transform.position).normalized;
        obj.transform.LookAt(LookAtDirection);

        yield return waitUseDelay;

        rb.velocity = dashDirection * data.speed;

        yield return waitduration;
        
        rb.velocity = Vector3.zero;

        yield return waitimmobilityTime;

        OnComplate();
        OnMoveCharacter(character);
    }
}
