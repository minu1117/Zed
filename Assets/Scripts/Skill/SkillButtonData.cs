using UnityEngine;

[CreateAssetMenu(fileName = "Skill Button Data", menuName = "Scriptable Objects/Skill Button Data")]
public class SkillButtonData : ScriptableObject
{
    public string keycode;
    public Skill skill;
    public int maxPoolSize;
    public Sprite sp;
}
