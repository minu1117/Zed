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

        // Target에 부딪혔을 경우 Target에 데미지 부여
        if (!isCollide && ReferenceEquals(target, obj))
        {
            isCollide = true;
            base.Collide(target);
        }

        // 관통하는 스킬일 경우 다른 대상에게 부딪혀도 데미지 부여
        else if (isPenetrate && !ReferenceEquals(target, obj))
        {
            base.Collide(obj);
        }
    }

    private IEnumerator CoShot()
    {
        float useTime = data.duration;
        Vector3 usePos = startPos;

        // 이동 거리 미리 계산 (적이 있는 방향으로)
        Vector3 totalMovement = usePos + (GetDir(usePos) * useTime * data.speed);
        totalMovement.y = usePos.y;

        for (var timePassed = 0f; timePassed < data.duration; timePassed += Time.deltaTime)
        {
            // Target에 부딪히지 않았을 경우 Target을 향해 이동 거리 및 방향 재계산
            if (!isCollide)
            {
                Vector3 dir = GetDir(usePos);
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
