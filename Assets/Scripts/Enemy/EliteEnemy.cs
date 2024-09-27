using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteEnemy : EnemyBase
{
    protected enum AttackMode
    {
        Normal,
        Combo,
    }

    [SerializeField] private ListManagerSkillButtonData comboSkillLists;
    private Dictionary<int, List<SkillButtonData>> comboDict;

    private float addWaitTime = 1f;
    private bool availableCombo;
    private float waitComboTime = 5f;
    private WaitForSeconds waitComboTimer;
    private Coroutine waitUseComboCoroutine;
    private Coroutine useComboCoroutine;

    private AttackMode attackMode;

    protected override void Awake()
    {
        base.Awake();
        AddComboSkill();
    }

    public override void Init()
    {
        base.Init();
        waitComboTimer = new WaitForSeconds(waitComboTime);
        attackMode = AttackMode.Normal;
        waitUseComboCoroutine = StartCoroutine(CoWaitUseCombo());
    }

    public override void Update()
    {
        base.Update();
        StateBehavior();
    }

    protected override void StateBehavior()
    {
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

    private void AttackByMode()
    {
        if (!availableCombo && waitUseComboCoroutine != null)
            attackMode = AttackMode.Normal;
        else
            attackMode = AttackMode.Combo;

        switch (attackMode)
        {
            case AttackMode.Normal:
                EnemyAttack();
                break;
            case AttackMode.Combo:
                UseComboSkill();
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

    private void AddComboSkill()
    {
        var listOfList = comboSkillLists.GetListOfLists();
        if (comboSkillLists == null || listOfList == null || listOfList.Count == 0)
            return;

        comboDict = new();
        var parent = slot.GetSlotObj();

        int comboName = 0;
        foreach (var list in listOfList)
        {
            var items = list.items;
            if (items == null || items.Count == 0)
                continue;

            foreach (var skillData in items)
            {
                slot.CreateExcutor(parent, skillData);
            }

            comboDict.Add(comboName++, items);
        }
    }
}
