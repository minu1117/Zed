using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    public KeyCode keycode;
    public TextMeshProUGUI tmp;
    private Image img;
    private SkillExcutor excutor;
    public SkillButtonData data;

    public void Init()
    {
        img = GetComponent<Image>();
        tmp.text = EnumConverter.GetString(keycode).ToUpper();
        SetSprite(data.sp);

        if (excutor == null)
            excutor = GetComponent<SkillExcutor>();

        excutor.Init(SkillSlotManager.Instance.gameObject, data);
    }

    public void SetSprite(Sprite sp)
    {
        if (img == null)
            return;

        img.sprite = sp;
    }

    public void SetData(SkillButtonData data)
    {
        this.data = data;
    }

    public ZedSkillType GetSkillType()
    {
        var zedSkillData = data as ZedSkillButtonData;
        if (zedSkillData == null)
            return ZedSkillType.None;

        return zedSkillData.type;
    }

    public SkillExcutor GetExcutor()
    {
        return excutor;
    }
}
