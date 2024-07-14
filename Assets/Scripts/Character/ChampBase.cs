using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChampBase : MonoBehaviour
{
    public CharacterData data;
    public AutoAttack autoAttack;
    public SkillSlot slot;
    public Transform shotStartTransform;
    public List<Weapon> weapons;
    private Dictionary<string, Weapon> weaponDict;
    protected CharacterAnimationController animationController;
    private HPController hpController;
    protected Coroutine coroutine;

    protected virtual void Awake()
    {
        hpController = GetComponent<HPController>();
        animationController = GetComponent<CharacterAnimationController>();
        weaponDict = new();

        if (weapons != null && weapons.Count > 0)
        {
            foreach (var weapon in weapons)
            {
                weapon.SetDamage(autoAttack.data.damage);
                weaponDict.Add(weapon.name, weapon);
            }
        }

        if (slot != null)
            slot.Init();
    }

    public void FinishedAttack()
    {
        if (weaponDict.Count == 0)
            return;

        foreach (var weapon in weaponDict)
        {
            weapon.Value.OnFinished();
        }
    }

    protected void OnAutoAttack(string name)
    {
        weaponDict[name].OnReady();
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

        if (hpController == null)
            return;

        hpController.SetCurrentHP(data.currentHp);
        if (data.currentHp <= 0)
            OnDead();
    }

    public virtual void OnDead()
    {
        Destroy(gameObject);
    }
}