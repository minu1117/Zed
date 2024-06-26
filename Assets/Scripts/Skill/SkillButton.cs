using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class SkillButton : MonoBehaviour
{
    private IObjectPool<Skill> skillPool;
    private GameObject poolObject;
    private SkillButtonData buttonData;
    public static int shadowID = 0;

    public void Init(SkillButtonData data)
    {
        if (data == null)
            return;

        buttonData = data;
        poolObject = new GameObject($"{buttonData.skill.data.skillName}");
        skillPool = new ObjectPool<Skill>
                    (
                        CreateSkill,
                        GetSkill,
                        ReleaseSkill,
                        DestroySkill,
                        maxSize : buttonData.maxPoolSize
                    );
    }

    public Skill StartSkill(GameObject character, string layerMask)
    {
        Vector3 point = Raycast.GetMousePointVec();
        point.y = character.transform.position.y;

        var useSkill = skillPool.Get();
        if (useSkill.data.type == SkillType.Dash)
        {
            var dashSkill = useSkill.GetComponent<DashSkill>();
            if (dashSkill == null)
                return null;

            dashSkill.SetPoint(point);
            dashSkill.Use(character);
            return dashSkill;
        }

        if (useSkill.isTargeting)
        {
            var findTarget = Raycast.FindMousePosTarget(layerMask);
            if (!findTarget.Item2)
            {
                skillPool.Release(useSkill);
                return null;
            }
            else
            {
                var targetingSkill = useSkill.GetComponent<TargetingSkill>();
                targetingSkill.SetTarget(findTarget.Item1);
            }    
        }

        if (useSkill.IsUsed())
            StartCoroutine(WaitUseSkill(useSkill, character, point));
        else
        {
            skillPool.Release(useSkill);
            return null;
        }

        return useSkill;
    }

    private IEnumerator WaitUseSkill(Skill useSkill, GameObject character, Vector3 lookAtPoint)
    {
        useSkill.SetActive(false);
        yield return new WaitForSeconds(useSkill.data.useDelay);

        Vector3 startPosition = character.gameObject.transform.position;
        if (character.TryGetComponent(out ChampBase champion))
        {
            startPosition = champion.shotStartTransform.position;
        }

        character.transform.LookAt(lookAtPoint);
        useSkill.SetActive(true);
        useSkill.SetPosition(startPosition);
        useSkill.SetStartPos(startPosition);
        useSkill.SetRotation(character.transform.rotation);

        if (buttonData.skill.data.isShadow && character.TryGetComponent(out Zed zed) && useSkill.TryGetComponent(out ZedShadow shadow))
        {
            useSkill.SetPosition(character.transform.position);

            if (shadow.GetID() <= 0)
            {
                shadowID++;
                shadow.SetID(shadowID);
            }

            zed.AddShadow(shadow);
        }

        useSkill.Use(character);
    }

    private Skill CreateSkill()
    {
        var useSkill = Instantiate(buttonData.skill, poolObject.transform);
        useSkill.SetPool(skillPool);

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

    public SkillButtonData GetData()
    {
        return buttonData;
    }

    public IObjectPool<Skill> GetPool() { return skillPool; }
}
