using UnityEngine;

[CreateAssetMenu(fileName = "Auto Attack Data", menuName = "Scriptable Objects/Auto Attack Data")]
public class AutoAttackData : ScriptableObject
{
    public float damage;
    public float attackSpeed;

    //[Header("시전 사운드")]
    //public List<AudioClip> useClips;

    //[Header("타격 사운드")]
    //public List<AudioClip> attackClips;
}
