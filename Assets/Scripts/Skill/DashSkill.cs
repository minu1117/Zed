using System.Collections;
using UnityEngine;

public class DashSkill : Skill
{
    private Vector3 movePoint;

    public override void Use(GameObject character)
    {
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
        
        if (obj.TryGetComponent<CharacterMoveController>(out var moveController))
            moveController.isMoved = false;

        rb.velocity = Vector3.zero;
        point.y = obj.transform.position.y;

        Vector3 LookAtDirection = (point == Vector3.zero) ? obj.transform.forward : point;
        Vector3 dashDirection = (point - obj.transform.position).normalized;
        obj.transform.LookAt(LookAtDirection);

        yield return waitUseDelay;

        if (rb != null)
            rb.velocity = dashDirection * data.speed;

        yield return waitduration;

        if (rb != null)
            rb.velocity = Vector3.zero;

        yield return waitimmobilityTime;

        movePoint = Vector3.zero;
        if (moveController != null)
            moveController.isMoved = true;

        Release();
    }
}
