using DG.Tweening;
using System.Collections;
using UnityEngine;

public class ShotSkill : Skill
{
    public override void Use(GameObject character)
    {
        if (!IsUsed())
            return;

        base.Use(character);
        StartCoroutine(CoShot(character.transform.forward));
    }

    private IEnumerator CoShot(Vector3 startVec)
    {
        // 나아갈 거리 미리 계산
        Vector3 totalMovement = transform.position + (startVec.normalized * data.duration * data.speed);

        transform.DOMove(totalMovement, data.duration)
                 .SetEase(Ease.Linear)
                 .OnComplete(() => ReleaseFunc());

        yield return waitimmobilityTime;
    }
}
