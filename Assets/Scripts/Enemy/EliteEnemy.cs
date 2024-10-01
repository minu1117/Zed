using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteEnemy : EnemyBase
{
    protected enum AttackMode
    {
        Normal,
        Combo,
        Pattern,
    }

    [SerializeField] protected ListManagerSkillButtonData comboSkillLists;
    protected Dictionary<int, List<SkillButtonData>> comboDict;

    private float addWaitTime = 1f;
    protected bool availableCombo;
    private float waitComboTime = 5f;
    private WaitForSeconds waitComboTimer;
    protected Coroutine waitUseComboCoroutine;
    private Coroutine useComboCoroutine;

    protected AttackMode attackMode;

    protected override void Awake()
    {
        base.Awake();
        comboDict = new();
        AddComboSkill(comboSkillLists, comboDict);
    }

    public override void Init()
    {
        base.Init();
        waitComboTimer = new WaitForSeconds(waitComboTime);
        attackMode = AttackMode.Normal;
        waitUseComboCoroutine = StartCoroutine(CoWaitUseCombo());   // 콤보 스킬 사용 대기
    }

    public override void Update()
    {
        base.Update();
        StateBehavior();
    }

    protected override void StateBehavior()
    {
        if (!availableCombo && waitUseComboCoroutine != null)
            attackMode = AttackMode.Normal;
        else
            attackMode = AttackMode.Combo;

        switch (state)
        {
            case State.Patrol:
                Patrol();
                break;
            case State.Chase:
                Chase();
                break;
            case State.Attack:
                AttackByMode();
                break;
        }
    }

    protected virtual void AttackByMode()
    {
        switch (attackMode)
        {
            case AttackMode.Normal:
                EnemyAttack();
                break;
            case AttackMode.Combo:
                UseComboSkill();
                break;
            default:
                break;
        }
    }

    private void UseComboSkill()
    {
        if (!availableCombo || waitUseComboCoroutine != null)
            return;

        availableCombo = false;
        StartRandomCombo();
    }

    private void StartRandomCombo()
    {
        var count = comboDict.Count;
        var randomRange = Random.Range(0, count);
        var combo = comboDict[randomRange];

        useComboCoroutine = StartCoroutine(CoUseCombo(combo));
    }

    private IEnumerator CoUseCombo(List<SkillButtonData> datas)
    {
        var slotDict = slot.GetSlotDict();
        var playerTag = EnumConverter.GetString(CharacterEnum.Player);

        foreach (var data in datas)
        {
            var slot = slotDict[data.keycode];
            slot.SetIsAvailable(true);

            var usedSkill = slot.StartSkill(gameObject, playerTag);
            var enemySkillButtonData = data as EnemySkillButtonData;
            animationController.UseSkill((int)enemySkillButtonData.type);
            yield return new WaitForSeconds(usedSkill.data.duration + addWaitTime);
        }

        useComboCoroutine = null;
        waitUseComboCoroutine = StartCoroutine(CoWaitUseCombo());
    }

    private IEnumerator CoWaitUseCombo()
    {
        yield return waitComboTimer;

        availableCombo = true;
        waitUseComboCoroutine = null;
    }

    protected void AddComboSkill(ListManagerSkillButtonData skillList, Dictionary<int, List<SkillButtonData>> dict)
    {
        var listOfList = skillList.GetListOfLists();
        if (skillList == null || listOfList == null || listOfList.Count == 0)
            return;

        int comboName = 0;
        foreach (var list in listOfList)
        {
            var items = list.items;
            if (items == null || items.Count == 0)
                continue;

            CreateNewSkills(items);
            dict.Add(comboName++, items);
        }
    }
}
