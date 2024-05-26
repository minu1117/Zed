using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.XR;

public abstract class Skill : MonoBehaviour, IDamageable
{
    public SkillData data;
    protected IObjectPool<Skill> pool;
    public abstract void Use(GameObject charactor);
    public void SetPool(IObjectPool<Skill> pool) { this.pool = pool; }

    public void DealDamage(DemoChampion target)
    {
        target.OnDamage(data.damage);
    }

    public void OnCollisionEnter(Collision collision)
    {
        TryDealDamage(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (data.isShadow)
            return;

        if (gameObject.TryGetComponent(out ZedShadow shadow))
        {
            if (!shadow.isReady)
                return;
        }

        TryDealDamage(other.gameObject);
    }

    private void TryDealDamage(GameObject target)
    {
        if (target.TryGetComponent(out DemoChampion champion))
        {
            if (champion.data.charactorName == "Zed")
                return;

            DealDamage(champion);
            Debug.Log($"{champion.data.charactorName} : {champion.data.currentHp}/{champion.data.maxhp}, {data.damage} Damage, {data.skillName}");
        }
    }
}
