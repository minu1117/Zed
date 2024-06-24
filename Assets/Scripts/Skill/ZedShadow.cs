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

    private Dictionary<string, List<KeyValuePair<IObjectPool<Skill>, KeyValuePair<KeyValuePair<Skill, ZedSkillType>, GameObject>>>> useSkills;
    private CharacterAnimationController animationController;

    public override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        animationController = GetComponent<CharacterAnimationController>();
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
                 .OnComplete(() => UseAllSkills());

        yield return new WaitForSeconds(data.duration);

        usePoint = Vector3.zero;
        zed.RemoveShadow(objectID);
        useSkills.Clear();

        if (pool != null)
        {
            isReady = false;
            agent.enabled = true;
            ReleaseFunc();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UseAllSkills()
    {
        isReady = true;
        usePoint = GetUsePoint();

        foreach (var skillPairList in useSkills)
        {
            foreach (var skillObject in skillPairList.Value)
            {
                var pool = skillObject.Key;

                var skill = skillObject.Value.Key.Key;
                var target = skillObject.Value.Value;
                var animationSkillType = skillObject.Value.Key.Value;

                UseCopySkill(skill, pool, target);
                StartAnimation(animationSkillType);
            }
        }

        useSkills.Clear();
    }

    public void AddSkill(string name, Skill skill, ZedSkillType type, IObjectPool<Skill> skillPool, GameObject target = null)
    {
        if (skill == null)
            return;

        if (isReady)
        {
            usePoint = GetUsePoint();
            UseCopySkill(skill, skillPool, target);
            StartAnimation(type);
            return;
        }

        //KeyValuePair<IObjectPool<Skill>, KeyValuePair<Skill, ZedSkillType>> pair = 
        //    new KeyValuePair<IObjectPool<Skill>, KeyValuePair<Skill, ZedSkillType>>(skillPool, new KeyValuePair<Skill, ZedSkillType>(skill, type));

        var pair =
            new KeyValuePair<IObjectPool<Skill>, KeyValuePair<KeyValuePair<Skill, ZedSkillType>, GameObject>> (skillPool,
            new KeyValuePair<KeyValuePair<Skill, ZedSkillType>, GameObject>(
            new KeyValuePair<Skill, ZedSkillType>(skill, type), target));

        if (skill.data.type == SkillType.Dash)
        {
            Vector3 point = Raycast.GetMousePointVec();
            dashSkill.SetPoint(point);
        }

        if (!useSkills.ContainsKey(name))
        {
            //List<KeyValuePair<IObjectPool<Skill>, KeyValuePair<Skill, ZedSkillType>>> pairList = new() { pair };
            List<KeyValuePair<IObjectPool<Skill>, KeyValuePair<KeyValuePair<Skill, ZedSkillType>, GameObject>>> pairList = new() { pair };
            useSkills.Add(name, pairList);
        }
        else
        {
            useSkills[name].Add(pair);
        }
    }

    private void StartAnimation(ZedSkillType type)
    {
        animationController.UseSkill((int)type);
    }

    private void UseCopySkill(Skill skill, IObjectPool<Skill> skillPool, GameObject target = null)
    {
        if (skill.data.type == SkillType.Dash)
        {
            UseCopyDash();
            return;
        }

        StartCoroutine(CoUseCopySkill(skill, skillPool, target));
    }

    private IEnumerator CoUseCopySkill(Skill skill, IObjectPool<Skill> skillPool, GameObject target = null)
    {
        if (skill.data.isShadow)
            yield break;

        yield return new WaitForSeconds(skill.data.useDelay);

        transform.LookAt(usePoint);

        var skillObject = skillPool.Get();
        if (skillObject.isTargeting)
        {
            if (target == null)
            {
                skillPool.Release(skillObject);
            }
            else
            {
                var targetingSkill = skillObject.GetComponent<TargetingSkill>();
                targetingSkill.SetTarget(target);
            }
        }

        skillObject.SetPool(skillPool);
        skillObject.SetPosition(shotStartTransform.position);
        skillObject.SetStartPos(shotStartTransform.position);
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
