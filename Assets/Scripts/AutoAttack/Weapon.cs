using UnityEngine;

public class Weapon : MonoBehaviour, IDamageable
{
    public WeaponData data;
    private bool isReady = false;
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
            DealDamage(champ, data.damage);
        }
    }

    public void OnReady()
    {
        isReady = true;
        coll.enabled = true;

        if (data.useClips == null || data.useClips.Count == 0)
            return;

        int randomIndex = Random.Range(0, data.useClips.Count);
        SoundManager.Instance.PlayOneShot(data.useClips[randomIndex]);
    }

    public void OnFinished()
    {
        isReady = false;
        coll.enabled = false;
    }

    public void SetDamage(float dmg)
    {
        data.damage = dmg;
    }

    public void DealDamage(ChampBase target, float damage)
    {
        target.OnDamage(damage);

        if (data.attackClips == null || data.attackClips.Count == 0)
            return;

        int randomIndex = Random.Range(0, data.attackClips.Count);
        SoundManager.Instance.PlayOneShot(data.attackClips[randomIndex]);
    }
}
