using UnityEngine;

[CreateAssetMenu(fileName = "Charactor Data", menuName = "Scriptable Objects/Charactor Data")]
public class CharacterData : ScriptableObject
{
    public float maxHp;
    public float currentHp;
    public float maxMp;
    public float currentMp;
    public int level;
    public string charactorName;
    public float moveSpeed;
    public float jumpSpeed;
    public float gravity;

    // Json Save & Load
}
