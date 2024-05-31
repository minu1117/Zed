using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class Skill : MonoBehaviour, IDamageable
{
    public SkillData data;
    protected IObjectPool<Skill> pool;
    protected bool isComplated = true;
    protected bool isCoolTime;
    protected bool isUsed;

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

    public void DealDamage(DemoChampion target)
    {
        target.OnDamage(data.damage);
    }

    public void OnCollisionEnter(Collision collision)
    {
        Collide(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        Collide(other.gameObject);
    }

    private void Collide(GameObject obj)
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

    private void DealDamage(GameObject target)
    {
        if (target.TryGetComponent(out DemoChampion champion))
        {
            if (champion.data.charactorName == "Zed")
                return;

            DealDamage(champion);
            Debug.Log($"{champion.data.charactorName} : {champion.data.currentHp}/{champion.data.maxhp}, {data.damage} Damage, {data.skillName}");
        }
    }

    protected IEnumerator CoCoolDown()
    {
        isCoolTime = true;
        yield return new WaitForSeconds(data.coolDown);
        isCoolTime = false;
    }

    protected Zed GetStopZed(GameObject character)
    {
        if (character.TryGetComponent(out Zed zed))
        {
            zed.StopMove();
            return zed;
        }
        else
        {
            return null;
        }
    }

    protected void OnComplate()
    {
        isUsed = false;
        isComplated = true;
    }

    protected void OnMoveZed(Zed zed)
    {
        if (zed != null)
            zed.isMoved = true;
    }

    protected void ReleaseFunc()
    {
        OnComplate();
        pool.Release(this);
    }
}
