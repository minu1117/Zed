using System.Collections;
using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    public string skillParamName;
    public string skillTriggerName;
    public string autoAttackTriggerName;
    public string attackSpeedParamName;
    public string attackTypeParamName;
    public string nextMotionTriggerParamName;
    public AutoAttackEnum maxAutoAttackEnum;
    private Animator animator;
    private int currentLayerIndex;
    private int upperLayerIndex = 1;
    private int wholeBodyLayerIndex = 2;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void UpdateMoveAnimation(Vector2 movement)
    {
        SetFloat("Horizontal",  movement.x);
        SetFloat("Vertical", movement.y);
    }

    public void Attack(float attackSpeed)
    {
        SetInteger(attackTypeParamName, Random.Range(0, (int)maxAutoAttackEnum + 1));
        SetFloat(attackSpeedParamName, attackSpeed);
        SetTrigger(autoAttackTriggerName);
    }

    public void StartNextMotion()
    {
        SetTrigger(nextMotionTriggerParamName);
    }

    public void UseSkill(int enumIndex)
    {
        bool isUpper = enumIndex != (int)ZedSkillType.ShadowRush ? true : false;
        currentLayerIndex = isUpper ? upperLayerIndex : wholeBodyLayerIndex;
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

    public float GetCurrentAnimLength()
    {
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(currentLayerIndex);
        return currentState.length;
    }
}
