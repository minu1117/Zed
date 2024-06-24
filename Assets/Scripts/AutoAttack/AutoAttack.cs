using UnityEngine;

public abstract class AutoAttack : MonoBehaviour, IDamageable
{
    public AutoAttackData data;
    public abstract void Attack(GameObject character);

    public void DealDamage(ChampBase target, float damage)
    {
        target.OnDamage(data.damage);
    }
}
