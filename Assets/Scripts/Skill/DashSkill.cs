using System.Collections;
using UnityEngine;

public class DashSkill : Skill
{
    private BoxCollider coll;
    private Vector3 movePoint;

    public override void Use(GameObject character)
    {
        base.Use(character);

        if (coll == null)
            coll = GetComponent<BoxCollider>();

        if (movePoint == null || movePoint == Vector3.zero)
            movePoint = Raycast.GetMousePointVec();

        SetColliderSize();
        StartCoroutine(CoDash(character, movePoint));
    }

    public void SetPoint(Vector3 point)
    {
        movePoint = point;
    }

    private void Update()
    {
        FollowCaster();
    }

    private void SetColliderSize()
    {
        if (caster == null)
            return;

        var casterCollider = caster.GetComponent<BoxCollider>();
        coll.size = casterCollider.size;
        coll.center = casterCollider.center;
    }

    private void FollowCaster()
    {
        if (caster != null)
        {
            gameObject.transform.position = caster.transform.position;
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        Collide(other.gameObject);
    }

    private IEnumerator CoDash(GameObject obj, Vector3 point)
    {
        var rb = obj.GetComponent<Rigidbody>();
        
        if (obj.TryGetComponent<CharacterMoveController>(out var moveController))
            moveController.isMoved = false;

        CharacterAnimationController animationController = null;
        if (obj.TryGetComponent<CharacterAnimationController>(out var controller))
            animationController = controller;

        rb.velocity = Vector3.zero;
        point.y = obj.transform.position.y;

        Vector3 LookAtDirection = (point == Vector3.zero) ? obj.transform.forward : point;
        Vector3 dashDirection = (point - obj.transform.position).normalized;
        obj.transform.LookAt(LookAtDirection);

        yield return waitUseDelay;

        if (animationController != null)
            animationController.StartNextMotion();

        if (rb != null)
            rb.velocity = dashDirection * data.speed;

        yield return waitduration;

        if (rb != null)
            rb.velocity = Vector3.zero;

        yield return waitimmobilityTime;

        if (animationController != null)
            animationController.StartNextMotion();

        movePoint = Vector3.zero;
        if (moveController != null)
            moveController.isMoved = true;

        Release();
    }
}
