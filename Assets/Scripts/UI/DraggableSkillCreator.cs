using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableSkillCreator : MonoBehaviour, IPointerEnterHandler
{
    public Canvas createdCanvas;
    public DraggableSkill draggableSkill;
    private Image img;
    private DraggableSkill createdDraggableSkill;
    private RectTransform rectTransform;
    private Canvas parentCanvas;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        img = GetComponent<Image>();
        parentCanvas = GetComponentInParent<Canvas>();

        Create();
    }

    private void MoveCurrentPos()
    {
        if (createdDraggableSkill == null)
            return;

        createdDraggableSkill.GetRectTransform().position = rectTransform.position;
    }

    private void Create()
    {
        createdDraggableSkill = Instantiate(draggableSkill);
        createdDraggableSkill.transform.SetParent(createdCanvas.transform);
        createdDraggableSkill.GetRectTransform().sizeDelta = rectTransform.sizeDelta;
        createdDraggableSkill.GetRectTransform().position = rectTransform.position;
        createdDraggableSkill.GetRectTransform().localScale = Vector3.one;
        createdDraggableSkill.SetCanvas(parentCanvas);

        img.sprite = createdDraggableSkill.GetImage().sprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (createdDraggableSkill.gameObject.activeSelf)
            return;

        MoveCurrentPos();
        createdDraggableSkill.gameObject.SetActive(true);
    }
}
