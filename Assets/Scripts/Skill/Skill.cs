using UnityEngine;
using UnityEngine.Pool;

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

    public virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out DemoChampion champion))
        {
            DealDamage(champion);
            Debug.Log($"{champion.data.charactorName} : {champion.data.currentHp}/{champion.data.maxhp}, {data.damage} Damage, {data.skillName}");
        }
    }
}
