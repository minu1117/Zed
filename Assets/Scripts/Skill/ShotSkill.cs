using DG.Tweening;
using System.Collections;
using UnityEngine;

public class ShotSkill : Skill
{
    public float speed;

    public override void Use(GameObject charactor)
    {
        StartCoroutine(CoShot(charactor, charactor.transform.forward));
    }

    private IEnumerator CoShot(GameObject charactor, Vector3 startVec)
    {
        Zed zed;
        if (charactor.TryGetComponent(out zed))
        {
            zed.isMoved = false;
        }

        float duration = data.duration;

        // 나아갈 거리 미리 계산
        Vector3 totalMovement = startVec.normalized * duration * speed;

        yield return new WaitForSeconds(data.useDelay);
        if (zed != null)
        {
            zed.isMoved = true;
        }

        transform.DOMove(transform.position + totalMovement, duration)
                 .SetEase(Ease.Linear)
                 .OnComplete(() => pool.Release(this));
    }
}
