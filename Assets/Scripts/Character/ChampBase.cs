using UnityEngine;

public class ChampBase : MonoBehaviour
{
    public CharacterData data;
    public SkillSlot slot;
    public Transform shotStartTransform;
    private HPController hpController;

    protected virtual void Awake() 
    {
        hpController = GetComponent<HPController>();
    }

    public HPController GetHPController() { return hpController; }

    public Skill UseSkill(string key)
    {
        if (slot == null)
            return null;

        var skillDict = slot.GetSlotDict();
        if (!skillDict.ContainsKey(key))
            return null;

        Skill skill = null;
        if (skillDict[key].skill.data.type != SkillType.Dash)
            skill = skillDict[key].StartSkill(gameObject);
        else
            skill = skillDict[key].OnDash(gameObject);

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
            OnDead();
        }

        hpController.SetCurrentHP(data.currentHp);
    }

    public virtual void OnDead()
    {
        Destroy(gameObject);
    }
}