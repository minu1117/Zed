using UnityEngine;
using UnityEngine.EventSystems;

public class SkillDropZone : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            var dragInfo = eventData.pointerDrag.GetComponent<DraggableSkill>();
            if (dragInfo == null)
                return;

            var skillButton = GetComponent<SkillButton>();
            if (skillButton == null)
                return;

            skillButton.SetData(dragInfo.skill);
            skillButton.Init();
        }
    }
}
