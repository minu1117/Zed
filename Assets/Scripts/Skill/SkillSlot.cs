using System.Collections.Generic;
using UnityEngine;

public class SkillSlot : Singleton<SkillSlot>
{
    //[SerializeField] private List<SkillButton> buttons;
    [SerializeField] private List<SkillButtonData> buttonDatas;
    [SerializeField] private SkillButton buttonPrefab;
    private Dictionary<string, SkillButton> slotDict;

    protected override void Awake()
    {
        base.Awake();

        if (buttonDatas == null || buttonDatas.Count == 0)
            return;

        if (buttonPrefab == null)
            return;

        slotDict = new Dictionary<string, SkillButton>();
        var slotObject = new GameObject("Skill_Slot");
        slotObject.transform.SetParent(transform, false);

        foreach (var buttonData in buttonDatas)
        {
            var button = Instantiate(buttonPrefab);
            button.transform.SetParent(slotObject.transform, false);
            button.Init(buttonData);

            slotDict.Add(buttonData.skillKey, button);
        }

        //slotDict = new Dictionary<string, SkillButton>();
        //foreach (var button in buttons)
        //{
        //    slotDict.Add(button.skillKey, button);
        //}
    }

    public Dictionary<string, SkillButton> GetSlotDict() { return slotDict; }

    //public void RenameKey(string currentKey, string newKey)
    //{
    //    if (slotDict.ContainsKey(newKey))
    //    {
    //        var removeKey = slotDict[newKey];
    //        removeKey.skillKey = string.Empty;
    //        RenameKey();
    //    }

    //    RenameKey(slotDict, currentKey, newKey);
    //}

    //private void RenameKey<TKey, TValue>(IDictionary<TKey, TValue> dic, TKey fromKey, TKey toKey)
    //{
    //    TValue value = dic[fromKey];
    //    dic.Remove(fromKey);
    //    dic[toKey] = value;
    //}
}
