using UnityEngine;

[CreateAssetMenu(fileName = "Skill Data", menuName = "Scriptable Objects/Skill Data")]
public class SkillData : ScriptableObject
{
    public SkillType type;
    public string skillName;
    public string skillDescription;
    public int skillLevel;
    public float damage;

    public float coolDown;
    public float duration;
    public float useDelay;
    public bool isShadow = false;
    public int levelRestriction;
    // ... etc

    // Json Save & Load
}
