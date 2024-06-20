using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    public string skillParamName;
    public string skillTriggerName;
    public string autoAttackTriggerName;
    public string attackSpeedParamName;
    public string attackTypeParamName;
    public AutoAttackEnum maxAutoAttackEnum;
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

    public void Attack(float attackSpeed)
    {
        SetInteger(attackTypeParamName, Random.Range(0, (int)maxAutoAttackEnum + 1));
        SetFloat(attackSpeedParamName, attackSpeed);
        SetTrigger(autoAttackTriggerName);
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

    public void SetInteger(string name, int value)
    {
        animator.SetInteger(name, value);
    }
}
