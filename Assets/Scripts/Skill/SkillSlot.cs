using System.Collections.Generic;
using UnityEngine;

public class SkillSlot : MonoBehaviour
{
    public List<SkillButtonData> buttonDatas;
    public SkillExcutor excutorPrefab;
    private Dictionary<string, SkillExcutor> slotDict;

    public void Init()
    {
        if (buttonDatas == null || buttonDatas.Count == 0)
            return;

        if (excutorPrefab == null)
            return;

        slotDict = new Dictionary<string, SkillExcutor>();
        var slotObject = new GameObject("Skill_Slot");
        slotObject.transform.SetParent(transform, false);

        foreach (var buttonData in buttonDatas)
        {
            var excutor = Instantiate(excutorPrefab);
            excutor.transform.SetParent(slotObject.transform, false);
            excutor.Init(gameObject, buttonData);

            if (buttonData.keycode == string.Empty)
            {
                buttonData.keycode = GetNotDuplicatedID();
            }

            slotDict.Add(buttonData.keycode, excutor);
        }
    }
    public Dictionary<string, SkillExcutor> GetSlotDict() { return slotDict; }
    
    private string GetNotDuplicatedID()
    {
        string randomID = GetRandomID().ToString();
        while (slotDict.ContainsKey(randomID))
        {
            randomID = GetRandomID().ToString();
        }

        return randomID;
    }

    private int GetRandomID()
    {
        return Random.Range(0, int.MaxValue);
    }
}
