using UnityEngine;

public class Weapon : MonoBehaviour, IDamageable
{
    private bool isReady = false;
    private float damage;
    private Collider coll;

    public void Awake()
    {
        coll = GetComponent<Collider>();
        coll.enabled = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!isReady)
            return;

        if (other.TryGetComponent(out ChampBase champ))
        {
            DealDamage(champ, damage);
        }
    }

    public void OnReady()
    {
        isReady = true;
        coll.enabled = true;
    }

    public void OnFinished()
    {
        isReady = false;
        coll.enabled = false;
    }

    public void SetDamage(float dmg)
    {
        damage = dmg;
    }

    public void DealDamage(ChampBase target, float damage)
    {
        target.OnDamage(damage);
    }
}
