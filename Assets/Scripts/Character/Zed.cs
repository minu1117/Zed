using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Zed : SingletonChampion<Zed>
{
    public Dictionary<int, ZedShadow> shadows = new();
    private CharacterAnimationController animationController;
    private ZedSkillType type;

    protected override void Awake()
    {
        base.Awake();
        animationController = GetComponent<CharacterAnimationController>();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Skill useSkill = UseSkill("Q");
            type = ZedSkillType.RazorShuriken;
            CopySkill("Q", useSkill, type, slot.GetSlotDict()["Q"].GetPool());
            animationController.UseSkill((int)type);
        }

        if (Input.GetKeyDown(KeyCode.F)) 
        {
            var hit = Raycast.GetHit(Input.mousePosition, "Shadow");
            if (hit.collider == null)
            {
                UseSkill("F");
                animationController.UseSkill((int)ZedSkillType.LivingShadow);
            }
            else
            {
                if (hit.collider.gameObject.TryGetComponent(out ZedShadow shadow))
                {
                    TeleportShadow(shadow);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            Skill useSkill = UseSkill("E");
            type = ZedSkillType.ShadowSlash;
            CopySkill("E", useSkill, type, slot.GetSlotDict()["E"].GetPool());
            animationController.UseSkill((int)type);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Skill useSkill = UseSkill("R");
            type = ZedSkillType.ShadowRush;
            CopySkill("R", useSkill, type, slot.GetSlotDict()["R"].GetPool());
            animationController.UseSkill((int)type);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            UseSkill("T");
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

    private void CopySkill(string skillKeyStr, Skill useSkill, ZedSkillType type, IObjectPool<Skill> skillPool)
    {
        if (useSkill == null)
            return;

        if (shadows.Count > 0)
        {
            foreach (var shadow in shadows)
            {
                shadow.Value.AddSkill(skillKeyStr, useSkill, type, skillPool);
            }
        }
    }

    public void TeleportShadow(ZedShadow shadow)
    {
        var hit = Raycast.GetHit(Input.mousePosition, "Shadow");
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
}
