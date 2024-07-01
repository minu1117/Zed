using UnityEngine;

public abstract class AutoAttack : MonoBehaviour, IDamageable
{
    public AutoAttackData data;
    public abstract void Attack(GameObject character);

    public void DealDamage(ChampBase target, float damage)
    {
        target.OnDamage(data.damage);

        int randomIndex = Random.Range(0, data.attackClips.Count);
        SoundManager.Instance.PlayOneShot(data.attackClips[randomIndex]);
    }
    
    protected void StartUseSound()
    {
        int randomIndex = Random.Range(0, data.useClips.Count);
        SoundManager.Instance.PlayOneShot(data.useClips[randomIndex]);
    }
}
