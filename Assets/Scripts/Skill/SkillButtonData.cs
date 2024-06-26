using UnityEngine;

[CreateAssetMenu(fileName = "Skill Button Data", menuName = "Scriptable Objects/Skill Button Data")]
public class SkillButtonData : ScriptableObject
{
    public string skillKey;
    public Skill skill;
    public int maxPoolSize;
}
