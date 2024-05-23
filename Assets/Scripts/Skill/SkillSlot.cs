using System.Collections.Generic;
using UnityEngine;

public class SkillSlot : MonoBehaviour
{
    [SerializeField] private List<SkillButton> buttons;
    private Dictionary<string, SkillButton> slotDict;

    public void Awake()
    {
        slotDict = new Dictionary<string, SkillButton>();
        foreach (var button in buttons)
        {
            slotDict.Add(button.skillKey, button);
        }
    }

    public List<SkillButton> GetButtons() { return buttons; }
    public Dictionary<string, SkillButton> GetSlotDict() { return slotDict; }

    // Dont Destroy
}
