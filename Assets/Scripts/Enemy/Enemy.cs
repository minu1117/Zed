using UnityEngine.AI;
using UnityEngine;
using UnityEngine.Pool;

public class Enemy : ChampBase
{
    private CharacterAnimationController animationController;
    private Rigidbody rb;
    private NavMeshAgent agent;
    private GameObject target;
    private IObjectPool<Enemy> pool;

    protected override void Awake()
    {
        base.Awake();
        slot = GetComponent<SkillSlot>();
        animationController = GetComponent<CharacterAnimationController>();
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        agent.speed = data.moveSpeed;
        SetTarget(FindAnyObjectByType<Zed>().gameObject);
    }

    public void SetTarget(GameObject targetObj)
    {
        target = targetObj;
    }

    public override void OnDead()
    {
        pool.Release(this);
    }

    public void SetPool(IObjectPool<Enemy> enemyPool)
    {
        pool = enemyPool;
    }

    public void Update()
    {
        agent.SetDestination(target.transform.position); 
    }
}
