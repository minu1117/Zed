using System.Collections.Generic;
using UnityEngine;

public class SkillSlot : Singleton<SkillSlot>
{
    [SerializeField] private List<SkillButton> buttons;
    private Dictionary<string, SkillButton> slotDict;

    protected override void Awake()
    {
        base.Awake();

        slotDict = new Dictionary<string, SkillButton>();
        foreach (var button in buttons)
        {
            slotDict.Add(button.skillKey, button);
        }
    }

    public List<SkillButton> GetButtons() { return buttons; }
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
