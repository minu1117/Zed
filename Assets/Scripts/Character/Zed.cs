using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class Zed : SingletonChampion<Zed>
{
    public Weapon L_Hand_Blade;
    public Weapon R_Hand_Blade;
    public Dictionary<int, ZedShadow> shadows = new();

    protected override void Awake()
    {
        base.Awake();
        if (L_Hand_Blade != null) L_Hand_Blade.SetDamage(autoAttack.data.damage);
        if (R_Hand_Blade != null) R_Hand_Blade.SetDamage(autoAttack.data.damage);
    }

    public void Update()
    {
        OnNoneTargetSkill(KeyCode.Q, ZedSkillType.RazorShuriken, "Q", true);
        OnNoneTargetSkill(KeyCode.E, ZedSkillType.ShadowSlash, "E", true);
        OnNoneTargetSkill(KeyCode.R, ZedSkillType.ShadowRush, "R", true);
        OnNoneTargetSkill(KeyCode.T, ZedSkillType.LivingShadow, "T", false);

        OnTargetingSkill(KeyCode.Y, ZedSkillType.RazorShuriken, "Y", true);

        OnShadowSkill(KeyCode.F, ZedSkillType.LivingShadow, "F");

        OnAutoAttack(MouseButton.Left);
    }

    private void OnAutoAttack(MouseButton mouseButton)
    {
        if (Input.GetMouseButtonDown((int)mouseButton))
        {
            FinishedAttack();
            Attack();
        }
    }

    private void OnNoneTargetSkill(KeyCode keyCode, ZedSkillType skillTypeEnum, string key, bool isCopy)
    {
        if (Input.GetKeyDown(keyCode))
        {
            Skill useSkill = UseSkill(key, EnumConverter.GetString(CharacterEnum.Enemy));

            if (isCopy)
                CopySkill(key, useSkill, skillTypeEnum, slot.GetSlotDict()[key].GetPool());

            if (useSkill != null)
                animationController.UseSkill((int)skillTypeEnum);
        }
    }

    private void OnTargetingSkill(KeyCode keyCode, ZedSkillType skillTypeEnum, string key, bool isCopy)
    {
        if (Input.GetKeyDown(keyCode))
        {
            Skill useSkill = UseSkill(key, EnumConverter.GetString(CharacterEnum.Enemy));
            var target = Raycast.FindMousePosTarget(EnumConverter.GetString(CharacterEnum.Enemy));

            if(isCopy)
                CopySkill(key, useSkill, skillTypeEnum, slot.GetSlotDict()[key].GetPool(), target.Item1);

            if (useSkill != null)
                animationController.UseSkill((int)skillTypeEnum);
        }
    }

    private void OnShadowSkill(KeyCode keyCode, ZedSkillType skillTypeEnum, string key)
    {
        if (Input.GetKeyDown(keyCode))
        {
            var hit = Raycast.GetHit(Input.mousePosition, EnumConverter.GetString(CharacterEnum.Shadow));
            if (hit.collider == null)
            {
                UseSkill(key, EnumConverter.GetString(CharacterEnum.Enemy));
                animationController.UseSkill((int)skillTypeEnum);
            }
            else
            {
                if (hit.collider.gameObject.TryGetComponent(out ZedShadow shadow))
                {
                    TeleportShadow(shadow);
                }
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
                shadow.Value.AddSkill(skillKeyStr, useSkill, type, skillPool, target);
            }
        }
    }

    public void TeleportShadow(ZedShadow shadow)
    {
        var hit = Raycast.GetHit(Input.mousePosition, EnumConverter.GetString(CharacterEnum.Shadow));
        if (hit.collider == null || !shadow.isReady)
            return;

        Vector3 position = gameObject.transform.position;
        Vector3 shadowPosition = shadow.transform.position;
        Quaternion rotation = transform.rotation;
        Quaternion shadowRotation = shadow.transform.rotation;

        gameObject.transform.position = shadowPosition;
        shadow.transform.position = position;
        gameObject.transform.rotation = shadowRotation;
        shadow.transform.rotation = rotation;
    }

    // Animation Event
    public void OnLeftAttack()
    {
        L_Hand_Blade.OnReady();
    }

    public void OnRightAttack()
    {
        R_Hand_Blade.OnReady();
    }

    public void FinishedAttack()
    {
        L_Hand_Blade.OnFinished();
        R_Hand_Blade.OnFinished();
    }
}
