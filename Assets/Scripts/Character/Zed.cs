using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class Zed : SingletonChampion<Zed>
{
    public GameObject L_Hand_Blade;
    public GameObject R_Hand_Blade;
    public Dictionary<int, ZedShadow> shadows = new();
    private SkillSlotManager skillSlotMgr;

    protected override void Awake()
    {
        base.Awake();
        skillSlotMgr = SkillSlotManager.Instance;
    }

    public void Update()
    {
        CheckUseSkill(KeyCode.Q, ZedSkillType.RazorShuriken, EnumConverter.GetString(KeyCode.Q));
        CheckUseSkill(KeyCode.E, ZedSkillType.ShadowSlash, EnumConverter.GetString(KeyCode.E));
        CheckUseSkill(KeyCode.R, ZedSkillType.ShadowRush, EnumConverter.GetString(KeyCode.R));
        CheckUseSkill(KeyCode.T, ZedSkillType.LivingShadow, EnumConverter.GetString(KeyCode.T));
        CheckUseSkill(KeyCode.G, ZedSkillType.RazorShuriken, EnumConverter.GetString(KeyCode.G));
        CheckUseSkill(KeyCode.F, ZedSkillType.LivingShadow, EnumConverter.GetString(KeyCode.F));

        CheckAutoAttack(MouseButton.Left);
    }

    public override Skill UseSkill(string key, string layerMask = "")
    {
        if (skillSlotMgr == null)
            return null;

        var skillDict = skillSlotMgr.GetSlotDict();
        if (!skillDict.ContainsKey(key))
            return null;

        Skill skill = skillDict[key].GetExcutor().StartSkill(gameObject, layerMask);
        return skill;
    }

    private void CheckAutoAttack(MouseButton mouseButton)
    {
        if (!Input.GetMouseButtonDown((int)mouseButton))
            return;

        FinishedAttack();
        Attack();
    }

    private void CheckUseSkill(KeyCode keyCode, ZedSkillType skillTypeEnum, string key)
    {
        if (!Input.GetKeyDown(keyCode))
            return;

        UseZedSkill(skillTypeEnum, key);
    }

    private void UseZedSkill(ZedSkillType skillTypeEnum, string key)
    {
        if (skillTypeEnum == ZedSkillType.LivingShadow)
        {
            UseShadowSkill(skillTypeEnum, key);
            return;
        }

        Skill useSkill = UseSkill(key, EnumConverter.GetString(CharacterEnum.Enemy));
        if (useSkill == null)
            return;

        FinishedAttack();

        (GameObject, bool) target = (null, false);
        if (useSkill.isTargeting)
        {
            target = Raycast.FindMousePosTarget(EnumConverter.GetString(CharacterEnum.Enemy));
        }

        //CopySkill(key, useSkill, skillTypeEnum, slot.GetSlotDict()[key].GetPool(), target.Item1);
        CopySkill(key, useSkill, skillTypeEnum, skillSlotMgr.GetSlotDict()[key].GetExcutor().GetPool(), target.Item1);

        animationController.UseSkill((int)skillTypeEnum);
        skillSlotMgr.CoolDown(useSkill.data.coolDown);
    }

    private void UseShadowSkill(ZedSkillType skillTypeEnum, string key)
    {
        var hit = Raycast.GetHit(Input.mousePosition, EnumConverter.GetString(CharacterEnum.Shadow));
        if (hit.collider == null)
        {
            FinishedAttack();
            Skill useSkill = UseSkill(key, EnumConverter.GetString(CharacterEnum.Enemy));

            if (useSkill != null)
            {
                animationController.UseSkill((int)skillTypeEnum);
                skillSlotMgr.CoolDown(useSkill.data.coolDown);
            }
        }
        else
        {
            if (hit.collider.gameObject.TryGetComponent(out ZedShadow shadow))
            {
                TeleportShadow(shadow);
            }
        }
    }

    public void AddShadow(ZedShadow shadow)
    {
        shadows.Add(shadow.GetID(), shadow);
    }

    public void RemoveShadow(int id)
    {
        shadows.Remove(id);
    }

    public Dictionary<int, ZedShadow> GetShadowDict()
    {
        return shadows;
    }    

    private void CopySkill(string skillKeyStr, Skill useSkill, ZedSkillType type, IObjectPool<Skill> skillPool, GameObject target = null)
    {
        if (useSkill == null)
            return;

        if (shadows.Count > 0)
        {
            foreach (var shadow in shadows)
            {
                shadow.Value.SetCaster(gameObject);
                shadow.Value.AddSkill(skillKeyStr, useSkill, type, skillPool, target);
            }
        }
    }

    public void TeleportShadow(ZedShadow shadow)
    {
        var hit = Raycast.GetHit(Input.mousePosition, EnumConverter.GetString(CharacterEnum.Shadow));
        if (hit.collider == null || !shadow.isReady)
            return;

        shadow.Teleport(gameObject);
    }

    // Animation Event
    public void OnLeftAttack()
    {
        OnAutoAttack(R_Hand_Blade.name);
    }

    public void OnRightAttack()
    {
        OnAutoAttack(L_Hand_Blade.name);
    }
}
