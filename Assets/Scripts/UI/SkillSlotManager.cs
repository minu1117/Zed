using System.Collections.Generic;
using UnityEngine;

public class SkillSlotManager : Singleton<SkillSlotManager>
{
    [SerializeField] private List<SkillButton> buttons;
    private Dictionary<string, SkillButton> buttonDict;

    protected override void Awake()
    {
        base.Awake();
        if (buttons == null || buttons.Count <= 0)
            return;

        buttonDict = new();
        foreach (var button in buttons)
        {
            button.Init();
            buttonDict.Add(EnumConverter.GetString(button.keycode), button);
        }
    }

    public void SetImage(string keycode, Sprite sp)
    {
        if (!buttonDict.ContainsKey(keycode))
            return;

        buttonDict[keycode].SetSprite(sp);
    }

    public void CoolDown(float time)
    {

    }

    public ZedSkillType GetType(string key)
    {
        if (!buttonDict.ContainsKey(key))
            return ZedSkillType.None;

        return buttonDict[key].GetSkillType();
    }

    public Dictionary<string, SkillButton> GetSlotDict()
    {
        return buttonDict;
    }
}
