using UnityEngine;
using UnityEngine.Pool;

public class Skill : MonoBehaviour, IDamageable
{
    public SkillData data;
    public bool isTargeting;
    protected IObjectPool<Skill> pool;
    protected Vector3 startPos;

    protected WaitForSeconds waitUseDelay;
    protected WaitForSeconds waitduration;
    protected WaitForSeconds waitimmobilityTime;

    private GameObject caster;

    public virtual void Awake()
    {
        waitUseDelay = new WaitForSeconds(data.useDelay);
        waitduration = new WaitForSeconds(data.duration);
        waitimmobilityTime = new WaitForSeconds(data.immobilityTime);
    }

    public virtual void Use(GameObject character) 
    {
        if (data.useClips == null || data.useClips.Count == 0)
            return;

        int index = GetRandomIndex(0, data.useClips.Count);
        SoundManager.Instance.PlayOneShot(data.useClips[index]);
    }

    public void SetPool(IObjectPool<Skill> pool) { this.pool = pool; }
    public void SetCaster(GameObject obj) { caster = obj; }
    public int GetRandomIndex(int min, int max) { return Random.Range(min, max); }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        Collide(collision.gameObject);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        Collide(other.gameObject);
    }

    protected virtual void Collide(GameObject obj)
    {
        if (data.isShadow)
            return;

        if (gameObject.TryGetComponent(out ZedShadow shadow))
        {
            if (!shadow.isReady)
                return;
        }

        DealDamage(obj);
    }

    public void SetStartPos(Vector3 pos)
    {
        startPos = pos;
    }

    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    public void SetRotation(Quaternion rot)
    {
        transform.rotation = rot;
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    public void DealDamage(ChampBase target, float damage)
    {
        target.OnDamage(damage);
    }

    private void DealDamage(GameObject target)
    {
        if (caster != null && ReferenceEquals(caster, target))
            return;

        if (target.TryGetComponent(out ChampBase champion))
        {
            DealDamage(champion, data.damage);

            if (data.attackClips == null || data.attackClips.Count == 0)
                return;

            int index = GetRandomIndex(0, data.attackClips.Count);
            SoundManager.Instance.PlayOneShot(data.attackClips[index]);
        }
    }

    protected virtual void Release()
    {
        if (pool == null)
            return;

        int index = GetRandomIndex(0, data.disappearClips.Count);
        SoundManager.Instance.PlayOneShot(data.disappearClips[index]);
        pool.Release(this);
    }
}
