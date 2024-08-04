using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    public string keycode;
    public TextMeshProUGUI tmp;
    private Image img;
    private SkillExcutor excutor;
    public SkillButtonData data;

    public void Init()
    {
        img = GetComponent<Image>();
        tmp.text = keycode.ToUpper();
        SetSprite(data.sp);

        excutor = GetComponent<SkillExcutor>();
        excutor.Init(SkillSlotManager.Instance.gameObject, data);
    }

    public void SetSprite(Sprite sp)
    {
        if (img == null)
            return;

        img.sprite = sp;
    }

    public SkillExcutor GetExcutor()
    {
        return excutor;
    }
}
