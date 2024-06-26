using UnityEngine;

public class ChampBase : MonoBehaviour
{
    public CharacterData data;
    public AutoAttack autoAttack;
    public SkillSlot slot;
    public Transform shotStartTransform;
    protected CharacterAnimationController animationController;
    private HPController hpController;

    protected virtual void Awake()
    {
        hpController = GetComponent<HPController>();
        animationController = GetComponent<CharacterAnimationController>();
    }

    public HPController GetHPController() { return hpController; }

    public void Attack()
    {
        animationController.Attack(autoAttack.data.attackSpeed);
        autoAttack.Attack(gameObject);
    }

    public Skill UseSkill(string key, string layerMask = "")
    {
        if (slot == null)
            return null;

        var skillDict = slot.GetSlotDict();
        if (!skillDict.ContainsKey(key))
            return null;

        Skill skill = skillDict[key].StartSkill(gameObject, layerMask);
        return skill;
    }

    public void OnDamage(float damage)
    {
        if (data.currentHp - damage >= 0)
        {
            data.currentHp -= damage;
        }
        else
        {
            data.currentHp = 0;
        }

        hpController.SetCurrentHP(data.currentHp);
        if (data.currentHp <= 0)
            OnDead();
    }

    public virtual void OnDead()
    {
        Destroy(gameObject);
    }
}