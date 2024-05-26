using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class DashSkill : Skill
{
    public float speed;

    public override void Use(GameObject charactor)
    {
        var hit = Raycast.GetHit(Input.mousePosition, "Enemy");
        if (hit.collider != null)
        {
            StartCoroutine(CoTargetDash(charactor, hit));
        }
        else
        {
            StartCoroutine(CoDash(charactor, Raycast.GetMousePointVec()));
        }
    }

    private IEnumerator CoTargetDash(GameObject charactor, RaycastHit hit)
    {
        Action action = null;
        if (charactor.TryGetComponent(out Zed zed))
        {
            action = GetZedMoveAction(zed);
        }

        float duration = data.duration;
        Vector3 point = hit.point;
        point.y = charactor.transform.position.y;
        charactor.transform.LookAt(point);

        // 나아갈 거리 미리 계산
        Vector3 totalMovement = charactor.transform.forward * duration * speed;
        totalMovement.x += totalMovement.x;
        totalMovement.z += totalMovement.z;

        yield return new WaitForSeconds(data.useDelay);

        transform.DOMove(charactor.transform.position + totalMovement, duration)
                 .SetEase(Ease.InOutBack)
                 .OnComplete(() => action.Invoke());
    }

    private IEnumerator CoDash(GameObject charactor, Vector3 point)
    {
        Action action = null;
        if (charactor.TryGetComponent(out Zed zed))
        {
            action = GetZedMoveAction(zed);
        }

        float duration = data.duration;
        point.y = charactor.transform.position.y;
        charactor.transform.LookAt(point);

        // 나아갈 거리 미리 계산
        Vector3 totalMovement = charactor.transform.forward * duration * speed;

        yield return new WaitForSeconds(data.useDelay);

        transform.DOMove(charactor.transform.position + totalMovement, duration)
                 .SetEase(Ease.InOutBack)
                 .OnComplete(() => action.Invoke());
    }

    private Action GetZedMoveAction(Zed zed)
    {
        zed.StopMove();
        Action action = () =>
        {
            if (zed != null)
            {
                zed.isMoved = true;
            }
        };

        return action;
    }
}
