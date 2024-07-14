using DG.Tweening;
using System.Collections;
using UnityEngine;

public class ShotSkill : Skill
{
    public TrailRenderer trailRenderer;
    public override void Use(GameObject character)
    {
        base.Use(character);
        SetActiveTrailRenderer(true);
        StartCoroutine(CoShot(character.transform.forward));
    }

    protected void SetActiveTrailRenderer(bool active)
    {
        if (trailRenderer == null)
            return;

        trailRenderer.Clear();
        trailRenderer.enabled = active;
    }

    private IEnumerator CoShot(Vector3 startVec)
    {
        // 나아갈 거리 미리 계산
        Vector3 totalMovement = transform.position + (startVec.normalized * data.duration * data.speed);

        transform.DOMove(totalMovement, data.duration)
                 .SetEase(Ease.Linear)
                 .OnComplete(() => Release());

        yield return waitimmobilityTime;
    }

    protected override void Release()
    {
        SetActiveTrailRenderer(false);

        if (pool == null)
            return;

        ReleaseEffect();
        StartDisappearSound();
        caster = null;
        pool.Release(this);
    }
}
