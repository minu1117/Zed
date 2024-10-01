using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : EliteEnemy
{
    [SerializeField] protected List<SkillButtonData> patternSkillList;
    protected Dictionary<float, List<SkillButtonData>> patternDict;
    private List<KeyValuePair<float, List<SkillButtonData>>> sortedPattern;
    private List<SkillButtonData> currentPattern;
    private float currentPatternHP;
    private float nextPatternHP;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Init()
    {
        base.Init();
        patternDict = new();
        AddPattern(patternSkillList, patternDict);
        SortPattern();
        SetCurrentPattern(sortedPattern[0].Value, sortedPattern[0].Key);
        nextPatternHP = data.maxHp;
        attackMode = AttackMode.Pattern;
    }

    public override void Update()
    {
        MoveAnimation();
        StateBehavior();
    }

    protected override void StateBehavior()
    {
        if (state == State.Patrol)
        {
            state = State.Chase;
            target = player;
        }

        //if (!availableCombo && waitUseComboCoroutine != null)
        //{
        //    attackMode = AttackMode.Normal;
        //}
        //else if ()
        //{
        //    attackMode = AttackMode.Combo;
        //}
        //else
        //{

        //}

        switch (state)
        {
            case State.Chase:
                Chase();
                break;
            case State.Attack:
                AttackByMode();
                break;
            default:
                break;
        }
    }

    protected override void AttackByMode()
    {
        base.AttackByMode();
        switch (attackMode)
        {
            case AttackMode.Pattern:
                UsePattern();
                break;
        }
    }

    private void UsePattern()
    {
        if (currentPattern == null || currentPattern.Count == 0)
            return;

        int randomIndex = Random.Range(0, currentPattern.Count);
        var playerTag = EnumConverter.GetString(CharacterEnum.Player);
        var data = currentPattern[randomIndex];

        var excutor = slot.GetSlotDict()[data.keycode];
        excutor.SetIsAvailable(true);
        excutor.StartSkill(gameObject, playerTag);

        var enemySkillButtonData = data as EnemySkillButtonData;
        animationController.UseSkill((int)enemySkillButtonData.type);
    }

    private void AddPattern(List<SkillButtonData> skillList, Dictionary<float, List<SkillButtonData>> dict)
    {
        CreateNewSkills(skillList);
        foreach (var skillData in skillList)
        {
            var bossSkill = skillData as BossSkillButtonData;
            if (bossSkill == null)
                continue;

            float percent = bossSkill.healthPercentage / 100f;
            float patternHP = data.maxHp * percent;

            if (dict.ContainsKey(patternHP))
            {
                dict[patternHP].Add(bossSkill);
            }
            else
            {
                var list = new List<SkillButtonData> { bossSkill };
                dict.Add(patternHP, list);
            }
        }
    }

    private void SortPattern()
    {
        sortedPattern = new();
        foreach (var item in patternDict)
        {
            sortedPattern.Add(item);
        }

        sortedPattern.Sort((KeyValuePair<float, List<SkillButtonData>> a, KeyValuePair<float, List<SkillButtonData>> b) => { return b.Key.CompareTo(a.Key); });
    }

    private void SetCurrentPattern(List<SkillButtonData> pattern, float patternHP)
    {
        currentPattern = pattern;
        currentPatternHP = patternHP;
    }

    private void DecidePattern()
    {
        float currentHP = data.currentHp;
        if (nextPatternHP < currentHP || currentHP <= 0)
            return;

        for (int i = 0; i < sortedPattern.Count; i++)
        {
            var key = sortedPattern[i].Key;
            if (key >= currentHP && key < currentPatternHP)
            {
                var value = sortedPattern[i].Value;
                SetCurrentPattern(value, key);

                if (i + 1 < sortedPattern.Count)
                {
                    nextPatternHP = sortedPattern[i + 1].Key;
                }
                else
                {
                    nextPatternHP = key;
                }
                break;
            }
        }
    }

    public override void OnDamage(float damage)
    {
        base.OnDamage(damage);
        DecidePattern();
    }
}
