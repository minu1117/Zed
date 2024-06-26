using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class Skill : MonoBehaviour, IDamageable
{
    public SkillData data;
    public bool isTargeting;
    protected IObjectPool<Skill> pool;
    protected bool isComplated = true;
    protected bool isCoolTime = false;
    protected bool isUsed;

    protected Vector3 startPos;

    protected WaitForSeconds waitUseDelay;
    protected WaitForSeconds waitduration;
    protected WaitForSeconds waitimmobilityTime;

    public virtual void Awake()
    {
        waitUseDelay = new WaitForSeconds(data.useDelay);
        waitduration = new WaitForSeconds(data.duration);
        waitimmobilityTime = new WaitForSeconds(data.immobilityTime);
    }

    public virtual void Use(GameObject character)
    {
        isComplated = false;
        isUsed = true;
        StartCoroutine(CoCoolDown());
    }

    public void SetPool(IObjectPool<Skill> pool) { this.pool = pool; }

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

        if (!isUsed)
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
        if (target.TryGetComponent(out ChampBase champion))
        {
            //if (champion.data.charactorName == EnumConverter.GetString(CharacterLayerEnum.Player))
            if (champion.gameObject.tag == EnumConverter.GetString(CharacterEnum.Player))
                return;

            DealDamage(champion, data.damage);
        }
    }

    protected IEnumerator CoCoolDown()
    {
        isCoolTime = true;
        yield return new WaitForSeconds(data.coolDown);
        isCoolTime = false;
    }

    protected void OnComplate()
    {
        isUsed = false;
        isComplated = true;
    }

    protected void ReleaseFunc()
    {
        OnComplate();

        if (pool == null) return;
        pool.Release(this);
    }

    public virtual bool IsUsed()
    {
        if (!isComplated || isCoolTime)
            return false;

        return true;
    }
}
