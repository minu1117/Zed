using UnityEngine.AI;
using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;

public class Enemy : ChampBase
{
    public float attackRange;
    public float skillRange;

    private Rigidbody rb;
    private NavMeshAgent agent;
    private GameObject target;
    private IObjectPool<Enemy> pool;

    private List<string> skillKeys;

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
        SetTarget(FindAnyObjectByType<Zed>().gameObject);
    }

    public void SetTarget(GameObject targetObj)
    {
        target = targetObj;
    }

    public override void OnDead()
    {
        if (pool != null)
            pool.Release(this);
        else
            Destroy(gameObject);
    }

    public void SetPool(IObjectPool<Enemy> enemyPool)
    {
        pool = enemyPool;
    }

    public void Update()
    {
        if (target == null)
            return;
            
        agent.SetDestination(target.transform.position);
        if (Vector3.Distance(transform.position, target.transform.position) <= attackRange)
        {
            if (skillKeys == null || skillKeys.Count == 0)
                return;

            StartRandomSkill();
        }
    }

    private void StartRandomSkill()
    {
        var count = skillKeys.Count;
        var randomIndex = Random.Range(0, count);
        var key = skillKeys[randomIndex];

        var slotDict = slot.GetSlotDict();
        var skillDistance = GetSkillDistance(slotDict, key);
        var distance = Vector3.Distance(transform.position, target.transform.position);

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

    private float GetSkillDistance(Dictionary<string, SkillButton> dict, string key)
    {
        return dict[key].GetData().skill.data.distance;
    }
}
