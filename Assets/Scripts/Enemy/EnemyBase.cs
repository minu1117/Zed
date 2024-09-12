using UnityEngine.AI;
using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;
using System.Collections;

public class EnemyBase : ChampBase
{
    public float recognitionRange;
    public float attackRange;
    public float skillRange;
    public float loseTargetTime;
    public float patrolRange;

    private Rigidbody rb;
    protected NavMeshAgent agent;
    protected GameObject target;
    protected GameObject player;
    private IObjectPool<EnemyBase> pool;

    protected List<string> skillKeys;
    private Coroutine loseTargetCoroutine;
    private Coroutine patrolCoroutine;
    private bool isPatrol;

    public void Init()
    {
        slot = GetComponent<SkillSlot>();
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        skillKeys = new();
        foreach (var skillButton in slot.GetSlotDict())
        {
            skillKeys.Add(skillButton.Key);
        }

        agent.speed = data.moveSpeed;
        player = FindFirstObjectByType<Zed>().gameObject;
        isPatrol = true;
    }

    public void SetIsPatrol(bool set)
    {
        isPatrol = set;
    }

    public void SetTarget(GameObject targetObj)
    {
        target = targetObj;
    }

    public override void OnDead()
    {
        if (pool != null)
        {
            foreach (var slot in slot.GetSlotDict())
            {
                slot.Value.SetIsAvailable(true);
            }

            target = null;
            pool.Release(this);
        }
        else
            Destroy(gameObject);
    }

    public void SetPool(IObjectPool<EnemyBase> enemyPool)
    {
        pool = enemyPool;
    }

    protected void StartRandomSkill()
    {
        var count = skillKeys.Count;
        var randomIndex = Random.Range(0, count);
        var key = skillKeys[randomIndex];

        var slotDict = slot.GetSlotDict();
        var skillDistance = GetSkillDistance(slotDict, key);
        var distance = GetDistance(target.transform.position);

        if (distance <= skillDistance)
        {
            slotDict[key].StartSkill(gameObject, EnumConverter.GetString(CharacterEnum.Player));
        }
        else
        {
            foreach (var skillButton in slot.GetSlotDict())
            {
                skillDistance = GetSkillDistance(slotDict, skillButton.Key);
                if (distance <= skillDistance)
                {
                    key = skillButton.Key;
                    slotDict[key].StartSkill(gameObject, EnumConverter.GetString(CharacterEnum.Player));
                    break;
                }
            }
        }
    }

    protected void UseRandomSkill()
    {
        if (target == null)
            return;

        if (GetDistance(target.transform.position) <= skillRange)
        {
            if (skillKeys == null || skillKeys.Count == 0)
                return;

            StartRandomSkill();
        }
    }

    private float GetSkillDistance(Dictionary<string, SkillExcutor> dict, string key)
    {
        return dict[key].GetData().skill.data.distance;
    }

    protected float GetDistance(Vector3 targetPos)
    {
        return Vector3.Distance(transform.position, targetPos);
    }

    protected void Chase()
    {
        if (target == null)
            return;

        agent.SetDestination(target.transform.position);
    }

    protected bool CheackRecognitionRange(Vector3 targetPos)
    {
        var distance = GetDistance(targetPos);
        if (distance <= recognitionRange)
            return true;

        return false;
    }

    protected void CheackLoseTarget()
    {
        if (target == null)
            return;

        if (CheackRecognitionRange(target.transform.position))
            return;

        target = null;
        isPatrol = true;
    }

    protected IEnumerator CoLoseTarget()
    {
        yield return new WaitForSeconds(loseTargetTime);
        CheackLoseTarget();
        loseTargetCoroutine = null;
    }

    protected void LoseTarget()
    {
        if (target != null && CheackRecognitionRange(target.transform.position))
        {
            if (loseTargetCoroutine != null)
            {
                StopCoroutine(loseTargetCoroutine);
                patrolCoroutine = null;
                loseTargetCoroutine = null;
                isPatrol = true;
            }

            return;
        }

        loseTargetCoroutine = StartCoroutine(CoLoseTarget());
    }

    protected void Patrol()
    {
        if (!isPatrol)
            return;

        if (patrolCoroutine == null)
            patrolCoroutine = StartCoroutine(CoPatrol());

        if (CheackRecognitionRange(player.transform.position))
        {
            if (patrolCoroutine != null)
            {
                StopCoroutine(patrolCoroutine);
                patrolCoroutine = null;
            }

            target = player;
            isPatrol = false;
        }
    }

    protected IEnumerator CoPatrol()
    {
        var randomPos = Random.insideUnitSphere * patrolRange;
        randomPos = transform.position + randomPos;
        randomPos.y = transform.position.y;
        agent.SetDestination(randomPos);

        yield return new WaitUntil(() => GetDistance(randomPos) < 5f);
        yield return new WaitForSeconds(1f);

        patrolCoroutine = null;
    }

#if UNITY_EDITOR

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;  // 원의 색상을 설정합니다.
        DrawCircle(gameObject.transform.position, recognitionRange);  // 원을 그립니다.
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
