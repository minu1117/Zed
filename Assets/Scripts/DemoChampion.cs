using UnityEngine;

public class DemoChampion : MonoBehaviour
{
    public CharacterData data;
    public SkillSlot slot;
    public Transform shotStartTransform;

    public virtual void Awake()
    {
        data.currentHp = data.maxhp;
    }

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
        }
    }
}