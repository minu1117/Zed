using System.Collections;
using System.Drawing;
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

    public Skill OnDash(GameObject character)
    {
        DashSkill dash = null;
        if (character.TryGetComponent(out DashSkill dashSkill))
        {
            dash = dashSkill;
            dash.Use(character);
        }

        return dash;
    }

    public Skill StartSkill(GameObject character)
    {
        Vector3 point = Raycast.GetMousePointVec();

        point.y = character.transform.position.y;

        var useSkill = skillPool.Get();
        useSkill.SetPool(skillPool);
        //useSkill.transform.position = startPosition;
        //useSkill.transform.rotation = character.transform.rotation;

        StartCoroutine(WaitUseSkill(useSkill, character, point));

        //useSkill.Use(character);
        return useSkill;
    }

    private IEnumerator WaitUseSkill(Skill useSkill, GameObject character, Vector3 lookAtPoint)
    {
        useSkill.gameObject.SetActive(false);

        yield return new WaitForSeconds(useSkill.data.useDelay);

        Vector3 startPosition = character.gameObject.transform.position;
        if (character.TryGetComponent(out DemoChampion champion))
        {
            startPosition = champion.shotStartTransform.position;
        }

        character.transform.LookAt(lookAtPoint);
        useSkill.gameObject.SetActive(true);
        useSkill.transform.position = startPosition;
        useSkill.transform.rotation = character.transform.rotation;

        if (skill.data.isShadow && character.TryGetComponent(out Zed zed) && useSkill.TryGetComponent(out ZedShadow shadow))
        {
            useSkill.transform.position = character.transform.position;

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
