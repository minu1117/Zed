using DG.Tweening;
using System.Collections;
using UnityEngine;

public class ShotSkill : Skill
{
    public TrailRenderer trailRenderer; // 따라다닐 TrailRenderer
    public override void Use(GameObject character)
    {
        base.Use(character);
        SetActiveTrailRenderer(true);   // TrailRenderer 활성화
        StartCoroutine(CoShot(character.transform.forward));    // 날리기 코루틴 실행
    }

    protected void SetActiveTrailRenderer(bool active)
    {
        if (trailRenderer == null)
            return;

        trailRenderer.Clear();
        trailRenderer.enabled = active;
    }

    // 날리기
    private IEnumerator CoShot(Vector3 startVec)
    {
        Vector3 totalMovement = transform.position + (startVec.normalized * data.duration * data.speed); // 날아갈 거리 계산

        transform.DOMove(totalMovement, data.duration)  // 지속 시간동안 totalMovement 까지 날아가기
                 .SetEase(Ease.Linear)
                 .OnComplete(() => Release());

        yield return waitimmobilityTime;    // 사용 후 경직 시간동안 대기
    }

    // 오브젝트 풀에 Release 하는 용도
    protected override void Release()
    {
        SetActiveTrailRenderer(false);  // TrailRenderer 비활성화

        if (pool == null)   // 오브젝트 풀이 없을 시 reutrn
            return;

        ReleaseEffect();        // 이펙트 Release
        StartDisappearSound();  // 사용 완료 사운드 재생
        caster = null;          // 시전자 제거
        pool.Release(this);     // 오브젝트 풀에 반납
    }
}
