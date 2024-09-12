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
    private List<KeyCode> keycodes;
    private RaycastedCanvas[] raycastedCanvases;

    protected override void Awake()
    {
        base.Awake();
        skillSlotMgr = SkillSlotManager.Instance;
        raycastedCanvases = FindObjectsOfType<RaycastedCanvas>();
    }

    private void Start()
    {
        keycodes = new();
        var slotDict = skillSlotMgr.GetSlotDict();
        foreach (var item in slotDict)
        {
            KeyCode code = (KeyCode)System.Enum.Parse(typeof(KeyCode), item.Key, true);
            keycodes.Add(code);
        }
    }

    public void Update()
    {
        foreach (var keycode in keycodes)
        {
            CheckUseSkill(keycode);
        }

        CheckAutoAttack(MouseButton.Left);
    }

    public override Skill UseSkill(string keycode, string layerMask = "")
    {
        if (skillSlotMgr == null)
            return null;

        var skillDict = skillSlotMgr.GetSlotDict();
        if (!skillDict.ContainsKey(keycode))
            return null;

        Skill skill = skillDict[keycode].GetExcutor().StartSkill(gameObject, layerMask);
        return skill;
    }

    private void CheckAutoAttack(MouseButton mouseButton)
    {
        if (!Input.GetMouseButtonDown((int)mouseButton))
            return;

        foreach (var canvas in raycastedCanvases)
        {
            if (canvas.IsHit())
                return;
        }

        FinishedAttack();
        Attack();
    }

    private void CheckUseSkill(KeyCode keyCode)
    {
        if (!Input.GetKeyDown(keyCode))
            return;

        string keycodeStr = EnumConverter.GetString(keyCode);
        ZedSkillType type = skillSlotMgr.GetType(keycodeStr);
        UseZedSkill(type, keycodeStr);
    }

    private void UseZedSkill(ZedSkillType type, string keycode)
    {
        if (type == ZedSkillType.LivingShadow)
        {
            UseShadowSkill(type, keycode);
            return;
        }

        Skill useSkill = UseSkill(keycode, EnumConverter.GetString(CharacterEnum.Enemy));
        if (useSkill == null)
            return;

        FinishedAttack();

        (GameObject, bool) target = (null, false);
        if (useSkill.isTargeting)
        {
            target = Raycast.FindMousePosTarget(EnumConverter.GetString(CharacterEnum.Enemy));
        }

        CopySkill(keycode, useSkill, type, skillSlotMgr.GetSlotDict()[keycode].GetExcutor().GetPool(), target.Item1);

        animationController.UseSkill((int)type);
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
