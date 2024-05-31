using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

public class ZedShadow : ShotSkill
{
    public bool isReady = false;
    private readonly float moveTime = 0.5f;
    public Transform shotStartTransform;
    private int objectID;
    public DashSkill dashSkill;
    private NavMeshAgent agent;
    private Rigidbody rb;

    public override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
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
        Vector3 point = Raycast.GetMousePointVec();

        agent.enabled = false;
        transform.forward = new Vector3(point.x, transform.position.y, point.z);

        yield return new WaitForSeconds(data.useDelay);

        transform.DOMove(point, moveTime)
                 .SetEase(Ease.OutQuad)
                 .OnComplete(() => isReady = true);

        yield return new WaitForSeconds(data.duration);

        zed.RemoveShadow(objectID);
        isReady = false;
        agent.enabled = true;
        pool.Release(this);
    }

    public void UseCopySkill(Skill skill, IObjectPool<Skill> skillPool)
    {
        StartCoroutine(CoUseCopySkill(skill, skillPool));
    }

    private IEnumerator CoUseCopySkill(Skill skill, IObjectPool<Skill> skillPool)
    {
        if (skill.data.isShadow || skill == null)
            yield break;

        Vector3 point = GetUsePoint();

        yield return new WaitForSeconds(skill.data.useDelay);
        yield return new WaitUntil(() => isReady == true);

        transform.LookAt(point);

        var skillObject = skillPool.Get();
        skillObject.SetPool(skillPool);
        skillObject.transform.position = shotStartTransform.position;
        skillObject.transform.rotation = transform.rotation;

        skillObject.Use(gameObject);
    }

    public void UseCopyDash()
    {
        StartCoroutine(CoUseCopyDash());
    }

    private IEnumerator CoUseCopyDash()
    {
        Vector3 point = GetUsePoint();

        yield return new WaitUntil(() => isReady == true);

        dashSkill.Use(gameObject);
    }

    private Vector3 GetUsePoint()
    {
        Vector3 point = Raycast.GetMousePointVec();
        point.y = transform.position.y;
        return point;
    }

    public void SetID(int id) { objectID = id; }
    public int GetID() { return objectID; }
}
