using System.Collections;
using UnityEngine;

public class TargetingShotSkill : TargetingSkill
{
    public bool isPenetrate;
    private bool isCollide = false;

    public override void Use(GameObject character)
    {
        if (!IsUsed() || target == null)
        {
            Release();
            return;
        }

        base.Use(character);
        StartCoroutine(CoShot());
    }

    protected override void OnTriggerEnter(Collider other)
    {
        Collide(other.gameObject);
    }

    protected override void Collide(GameObject obj)
    {
        if (target == null || obj == null)
            return;

        if (!isCollide && ReferenceEquals(target, obj))
        {
            isCollide = true;
            base.Collide(target);
        }
        else if (isPenetrate && !ReferenceEquals(target, obj))
        {
            base.Collide(obj);
        }
    }

    private IEnumerator CoShot()
    {
        float useTime = data.duration;
        Vector3 usePos = startPos;

        Vector3 totalMovement = usePos + (GetDir(usePos) * useTime * data.speed);
        totalMovement.y = usePos.y;
        for (var timePassed = 0f; timePassed < data.duration; timePassed += Time.deltaTime)
        {
            Vector3 dir = GetDir(usePos);

            if (!isCollide)
            {
                totalMovement = usePos + (dir * useTime * data.speed);
                totalMovement.y = usePos.y;
            }

            var factor = timePassed / data.duration;
            transform.position = Vector3.Lerp(usePos, totalMovement, factor);
            yield return null;
        }

        Release();
    }

    private Vector3 GetDir(Vector3 usePosition)
    {
        Vector3 dir = target.transform.position - usePosition;
        dir.y = usePosition.y;
        dir.Normalize();

        return dir;
    }

    private void Release()
    {
        ReleaseFunc();
        target = null;
        isCollide = false;
    }
}
