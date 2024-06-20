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

    public void DealDamage(ChampBase target, float damage)
    {
        target.OnDamage(damage);
    }

    public void OnCollisionEnter(Collision collision)
    {
        Collide(collision.gameObject);
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
        if (target.TryGetComponent(out ChampBase champion))
        {
            if (champion.data.charactorName == "Zed")
                return;

            DealDamage(champion, data.damage);
            //Debug.Log($"{champion.data.charactorName} : {champion.data.currentHp}/{champion.data.maxhp}, {data.damage} Damage, {data.skillName}");
        }
    }

    protected IEnumerator CoCoolDown()
    {
        isCoolTime = true;
        yield return new WaitForSeconds(data.coolDown);
        isCoolTime = false;
    }

    protected CharacterMoveController GetStopCharacter(GameObject obj)
    {
        if (obj.TryGetComponent(out CharacterMoveController character))
        {
            character.StopMove();
            return character;
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

    protected void OnMoveCharacter(CharacterMoveController character)
    {
        if (character != null)
            character.isMoved = true;
    }

    protected void ReleaseFunc()
    {
        OnComplate();

        if (pool == null) return;
        pool.Release(this);
    }
}
