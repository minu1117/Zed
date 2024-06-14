using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    public string skillParamName;
    public string skillTriggerName;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void UpdateMoveAnimation(Vector2 movement)
    {
        SetFloat("Horizontal", movement.x);
        SetFloat("Vertical", movement.y);
    }

    public void UseSkill(int enumIndex)
    {
        bool isUpper = enumIndex != (int)ZedSkillType.ShadowRush ? true : false;
        animator.SetBool("IsUpper", isUpper);

        animator.SetInteger(skillParamName, enumIndex);
        animator.SetTrigger("UseSkill");
    }

    public void SetTrigger(string triggerName)
    {
        animator.SetTrigger(triggerName);
    }

    public void SetFloat(string name, float value)
    {
        animator.SetFloat(name, value);
    }

    public void SetBool(string name, bool value)
    {
        animator.SetBool(name, value);
    }
}
