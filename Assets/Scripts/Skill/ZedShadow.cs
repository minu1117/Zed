using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

public class ZedShadow : ShotSkill
{
    private readonly float moveTime = 0.5f;
    private int objectID;

    public bool isReady = false;
    public Transform shotStartTransform;
    public DashSkill dashSkill;

    private NavMeshAgent agent;
    private Rigidbody rb;
    private Vector3 usePoint;

    private Dictionary<string, List<KeyValuePair<IObjectPool<Skill>, Skill>>> useSkills;
    private Coroutine useCoroutine;

    public override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        useSkills = new();
    }

    public NavMeshAgent GetAgent() { return agent; }

    public override void Use(GameObject charactor)
    {
        if (charactor.TryGetComponent(out Zed zed))
        {
            StartCoroutine(CoSpawnShadow(zed));
        }
    }

    private IEnumerator CoSpawnShadow(Zed zed)
    {
        if (usePoint == null || usePoint == Vector3.zero)
            usePoint = Raycast.GetMousePointVec();

        agent.enabled = false;
        transform.forward = new Vector3(usePoint.x, transform.position.y, usePoint.z);

        yield return new WaitForSeconds(data.useDelay);

        transform.DOMove(usePoint, moveTime)
                 .SetEase(Ease.OutQuad)
                 .OnComplete(() => useCoroutine = StartCoroutine(UseAllSkills()));

        yield return new WaitForSeconds(data.duration);

        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        zed.RemoveShadow(objectID);
        useSkills.Clear();

        if (pool != null)
        {
            isReady = false;
            agent.enabled = true;
            useCoroutine = null;
            ReleaseFunc();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public IEnumerator UseAllSkills()
    {
        isReady = true;
        usePoint = GetUsePoint();

        foreach (var skillPairList in useSkills)
        {
            foreach (var skillObject in skillPairList.Value)
            {
                var skill = skillObject.Key.Get();
                if (skill.data.isShadow || skill == null)
                    continue;

                transform.LookAt(usePoint);

                yield return new WaitForSeconds(skill.data.useDelay);

                skill.SetPool(skillObject.Key);
                skill.SetPosition(shotStartTransform.position);
                skillObject.Value.SetRotation(transform.rotation);
                skill.Use(gameObject);
            }
        }

        useCoroutine = null;
        useSkills.Clear();
    }

    public void AddSkill(string name, Skill skill, IObjectPool<Skill> skillPool)
    {
        if (!isReady)
        {
            KeyValuePair<IObjectPool<Skill>, Skill> pair = new KeyValuePair<IObjectPool<Skill>, Skill>(skillPool, skill);

            if (!useSkills.ContainsKey(name))
            {
                List<KeyValuePair<IObjectPool<Skill>, Skill>> pairList = new() { pair };
                useSkills.Add(name, pairList);
            }
            else
            {
                useSkills[name].Add(pair);
            }
        }
        else
        {
            UseCopySkill(skill, skillPool);
        }
    }

    private void UseCopySkill(Skill skill, IObjectPool<Skill> skillPool)
    {
        StartCoroutine(CoUseCopySkill(skill, skillPool));
    }

    private IEnumerator CoUseCopySkill(Skill skill, IObjectPool<Skill> skillPool)
    {
        if (skill.data.isShadow || skill == null)
            yield break;

        usePoint = GetUsePoint();

        yield return new WaitForSeconds(skill.data.useDelay);
        yield return new WaitUntil(() => isReady == true);

        transform.LookAt(usePoint);

        var skillObject = skillPool.Get();
        skillObject.SetPool(skillPool);
        skillObject.SetPosition(shotStartTransform.position);
        skillObject.SetRotation(transform.rotation);

        skillObject.Use(gameObject);
    }

    public void UseCopyDash()
    {
        StartCoroutine(CoUseCopyDash());
    }

    private IEnumerator CoUseCopyDash()
    {
        yield return new WaitUntil(() => isReady == true);
        dashSkill.Use(gameObject);
    }

    private Vector3 GetUsePoint()
    {
        Vector3 point = Raycast.GetMousePointVec();
        point.y = transform.position.y;
        return point;
    }

    public void SetPoint(Vector3 point) { usePoint = point; }
    public void SetID(int id) { objectID = id; }
    public int GetID() { return objectID; }
}
