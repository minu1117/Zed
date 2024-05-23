using UnityEngine;

[CreateAssetMenu(fileName = "Charactor Data", menuName = "Scriptable Objects/Charactor Data")]
public class CharactorData : ScriptableObject
{
    public float maxhp;
    public float currentHp;
    public int level;
    public string charactorName;
    public float moveSpeed;

    // Json Save & Load
}
