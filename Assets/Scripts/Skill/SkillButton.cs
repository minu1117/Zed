using UnityEngine;
using UnityEngine.Pool;

public class SkillButton : MonoBehaviour
{
    public string skillKey;
    public Skill skill;
    private IObjectPool<Skill> skillPool;
    private int shadowID = 0;
    private GameObject poolObject;

    public void Awake()
    {
        poolObject = new GameObject($"{skill.data.skillName}");
        skillPool = new ObjectPool<Skill>
                    (
                        CreateSkill,
                        GetSkill,
                        ReleaseSkill,
                        DestroySkill
                    );
    }

    public Skill OnDash(GameObject charactor)
    {
        DashSkill dash = null;
        if (charactor.TryGetComponent(out DashSkill dashSkill))
        {
            dash = dashSkill;
            dash.Use(charactor);
        }

        return dash;
    }

    public Skill StartSkill(GameObject charactor, Vector3 startPosition)
    {
        Vector3 point = Raycast.GetMousePointVec();

        point.y = charactor.transform.position.y;
        charactor.transform.LookAt(point);

        var useSkill = skillPool.Get();
        useSkill.SetPool(skillPool);
        useSkill.transform.position = startPosition;
        useSkill.transform.rotation = charactor.transform.rotation;

        if (skill.data.isShadow && charactor.TryGetComponent(out Zed zed) && useSkill.TryGetComponent(out ZedShadow shadow))
        {
            useSkill.transform.position = charactor.transform.position;

            if (shadow.GetID() <= 0)
            {
                shadowID++;
                shadow.SetID(shadowID);
            }

            zed.AddShadow(shadow);
        }

        useSkill.Use(charactor);
        return useSkill;
    }

    private Skill CreateSkill()
    {
        var useSkill = Instantiate(skill, poolObject.transform);
        return useSkill;
    }
    private void GetSkill(Skill skill)
    {
        skill.gameObject.SetActive(true);
    }
    private void ReleaseSkill(Skill skill)
    {
        skill.gameObject.SetActive(false);
    }
    private void DestroySkill(Skill skill)
    {
        Destroy(skill.gameObject);
    }

    public IObjectPool<Skill> GetPool() { return skillPool; }
    // Dont Destroy
}
