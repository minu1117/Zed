using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleShadow : Skill
{
    public int shadowCount;
    public ZedShadow shadowSkill;
    public float interval;
    public float individualWaitDelay;
    private WaitForSeconds waitDelay;

    public override void Awake()
    {
        base.Awake();
        data.duration = shadowSkill.data.duration + 1f;
        waitDelay = new WaitForSeconds(individualWaitDelay);
    }

    public override void Use(GameObject character)
    {
        base.Use(character);
        StartCoroutine(CoSpawnMultipleShadow(character));
    }

    private IEnumerator CoSpawnMultipleShadow(GameObject character)
    {
        List<ZedShadow> shadows = new List<ZedShadow>();
        for (int i = 0; i < shadowCount; i++)
        {
            var shadow = Instantiate(shadowSkill, gameObject.transform);
            shadow.SetActive(false);
            shadows.Add(shadow);
        }

        yield return new WaitForSeconds(shadowSkill.data.useDelay);

        if (character.TryGetComponent(out Zed zed))
        {
            Vector3 startPosition = character.gameObject.transform.position;
            Vector3 startDirection = character.gameObject.transform.forward;

            for (int i = 0; i < shadows.Count; i++)
            {
                int id = ++SkillButton.shadowID;
                shadows[i].SetID(id);

                float angle = (360f / shadowCount) * i;
                Vector3 rotatedDirection = Quaternion.AngleAxis(angle, Vector3.up) * startDirection;
                Vector3 point = startPosition + rotatedDirection * interval;

                shadows[i].SetPosition(startPosition);
                shadows[i].SetPoint(point);
                zed.AddShadow(shadows[i]);
            }

            foreach (var shadow in shadows)
            {
                shadow.SetActive(true);
                shadow.Use(character);

                yield return waitDelay;
            }
        }

        yield return new WaitForSeconds(data.duration);

        ReleaseFunc();
    }
}
