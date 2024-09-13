using UnityEngine.AI;
using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;
using System.Collections;

public class EnemyBase : ChampBase
{
    public float recognitionRange;          // 타겟 인식 범위
    public float attackRange;               // 일반 공격 범위
    public float skillRange;                // 스킬 사용 범위
    public float loseTargetTime;            // 타겟 해제 시간
    public float patrolRange;               // 정찰 범위

    private Rigidbody rb;
    protected NavMeshAgent agent;
    protected GameObject target;
    protected GameObject player;
    private IObjectPool<EnemyBase> pool;

    protected List<string> skillKeys;
    private Coroutine loseTargetCoroutine;
    private Coroutine patrolCoroutine;
    private bool isPatrol;

    // 초기 설정
    public void Init()
    {
        slot = GetComponent<SkillSlot>();
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        skillKeys = new();  // 스킬 키 List 초기화
        foreach (var skillButton in slot.GetSlotDict()) // 스킬 슬롯 순회, 스킬 키 List에 key 추가
        {
            skillKeys.Add(skillButton.Key);
        }

        agent.speed = data.moveSpeed;
        player = FindFirstObjectByType<Zed>().gameObject;   // 플레이어 오브젝트 미리 담아두기 (타겟 설정 시 사용)
        isPatrol = true;                                    // 정찰 행동 활성화
    }

    // 정찰 행동 여부 설정
    public void SetIsPatrol(bool set)
    {
        isPatrol = set;
    }

    // 타겟 설정
    public void SetTarget(GameObject targetObj)
    {
        target = targetObj;
    }

    public override void OnDead()
    {
        // 오브젝트 풀이 설정된 경우
        if (pool != null)
        {
            // 스킬 슬롯을 순회하며 모든 스킬을 사용 가능한 상태로 변경
            foreach (var slot in slot.GetSlotDict())
            {
                slot.Value.SetIsAvailable(true);
            }

            target = null;          // 타겟 해제
            pool.Release(this);     // 오브젝트 풀에 본인 반납
        }

        // 오브젝트 풀에 들어가지 않았을 경우
        else
            Destroy(gameObject);    // 그냥 삭제
    }

    // 오브젝트 풀 설정 (죽었을 때 반납용)
    public void SetPool(IObjectPool<EnemyBase> enemyPool)
    {
        pool = enemyPool;
    }

    // 랜덤 스킬 실행 메서드
    protected void StartRandomSkill()
    {
        var count = skillKeys.Count;                            // 담아둔 key들의 개수
        var randomIndex = Random.Range(0, count);               // 랜덤 인덱스
        var key = skillKeys[randomIndex];                       // key를 담아둔 List에서 랜덤 인덱스의 key 가져오기

        var slotDict = slot.GetSlotDict();                      // 스킬 슬롯 dictionary
        var skillDistance = GetSkillDistance(slotDict, key);    // 실행할 스킬의 거리 가져오기
        var distance = GetDistance(target.transform.position);  // 타겟과의 거리

        // 처음 랜덤으로 가져온 스킬의 범위 검사
        // 타겟이 스킬 범위 안에 있을 경우
        if (distance <= skillDistance)
        {
            slotDict[key].StartSkill(gameObject, EnumConverter.GetString(CharacterEnum.Player));    // 스킬 실행
        }

        // 랜덤으로 가져온 스킬의 범위에 타겟이 없을 경우
        else
        {
            // 스킬 슬롯을 순회하며 범위가 닿는 스킬 찾기
            foreach (var skillButton in slotDict)
            {
                skillDistance = GetSkillDistance(slotDict, skillButton.Key);
                if (distance <= skillDistance)  // 가져온 스킬의 범위가 닿을 경우 스킬 실행, 반복문 중단
                {
                    key = skillButton.Key;
                    slotDict[key].StartSkill(gameObject, EnumConverter.GetString(CharacterEnum.Player));
                    break;
                }
            }
        }
    }

    // 랜덤 스킬 실행 여부 확인 메서드
    protected void UseRandomSkill()
    {
        if (target == null)
            return;

        if (GetDistance(target.transform.position) <= skillRange)   // 타겟이 스킬 범위 안에 있을 경우
        {
            if (skillKeys == null || skillKeys.Count == 0)  // 스킬 슬롯이 비었을 경우 중지
                return;

            StartRandomSkill(); // 가지고 있는 스킬 중 랜덤으로 골라 스킬 실행
        }
    }

    // 가지고 있는 스킬의 거리 return
    private float GetSkillDistance(Dictionary<string, SkillExcutor> dict, string key)
    {
        return dict[key].GetData().skill.data.distance;
    }

    // 거리 재기
    protected float GetDistance(Vector3 targetPos)
    {
        return Vector3.Distance(transform.position, targetPos);
    }

    // 추적 행동 메서드
    protected void Chase()
    {
        if (target == null)
            return;

        agent.SetDestination(target.transform.position);
    }

    // 타겟 위치 재확인, 타겟 해제 메서드
    protected void CheackLoseTarget()
    {
        if (target == null)
            return;

        if (GetDistance(target.transform.position) <= recognitionRange) // 타겟이 인식 범위 밖에 있을 경우
            return;

        target = null;  // 타겟 해제
        isPatrol = true;    // 정찰 시작
    }

    // 타겟 해제 코루틴
    protected IEnumerator CoLoseTarget()
    {
        yield return new WaitForSeconds(loseTargetTime);    // 타겟 해제 타이머 시간동안 대기
        CheackLoseTarget(); // 타겟 위치 재확인
        loseTargetCoroutine = null;
    }

    // 타겟 해제 메서드
    protected void LoseTarget()
    {
        if (target != null && GetDistance(target.transform.position) <= recognitionRange)   // 타겟 설정이 안 되어있고, 타겟이 인식 범위에 있을 경우
        {
            if (loseTargetCoroutine != null)    // 타겟 해제 코루틴이 실행 중일 때
            {
                // 정찰 코루틴 정지, 타겟 해제 코루틴 정지 및 초기화, 정찰 활성화
                StopCoroutine(loseTargetCoroutine);
                patrolCoroutine = null;
                loseTargetCoroutine = null;
                isPatrol = true;
            }

            return;
        }

        loseTargetCoroutine = StartCoroutine(CoLoseTarget());   // 타겟 해제 코루틴 시작
    }

    // 정찰 행동 메서드
    protected void Patrol()
    {
        if (!isPatrol)
            return;

        if (patrolCoroutine == null)                                    // 코루틴이 실행되지 않았을 경우
            patrolCoroutine = StartCoroutine(CoPatrol());               // 코루틴 시작

        if (GetDistance(player.transform.position) <= recognitionRange) // 플레이어가 인식 범위 안에 들어왔을 경우
        {
            if (patrolCoroutine != null)                                // 코루틴이 실행 중일 때 코루틴 중지
            {
                StopCoroutine(patrolCoroutine);
                patrolCoroutine = null;
            }

            target = player;    // 타겟 설정 (플레이어)
            isPatrol = false;   // 정찰 중지
        }
    }

    // 범위 내 랜덤 위치로 이동하는 코루틴
    protected IEnumerator CoPatrol()
    {
        var randomPos = Random.insideUnitSphere * patrolRange;          // patrolRange로 범위 조절
        randomPos = transform.position + randomPos;                     // 현재 위치를 기반으로 범위 설정
        randomPos.y = transform.position.y;                             // y값이 위, 아래로 크게 변동되지 않게 랜덤 위치값을 현재 캐릭터 위치의 y값으로 변경
        agent.SetDestination(randomPos);

        yield return new WaitUntil(() => GetDistance(randomPos) < 5f);  // 위치에 도착할 때 까지 대기
        yield return new WaitForSeconds(1f);                            // 도착 후 1초 대기

        patrolCoroutine = null;
    }

#if UNITY_EDITOR

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        DrawCircle(gameObject.transform.position, recognitionRange);    // 플레이어 인식 범위
    }

    void DrawCircle(Vector3 position, float radius)
    {
        int segments = 100;
        float angle = 0f;

        for (int i = 0; i < segments; i++)
        {
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            angle += 2 * Mathf.PI / segments;

            float nextX = Mathf.Cos(angle) * radius;
            float nextZ = Mathf.Sin(angle) * radius;

            Gizmos.DrawLine(position + new Vector3(x, 0, z), position + new Vector3(nextX, 0, nextZ));
        }
    }

#endif
}
