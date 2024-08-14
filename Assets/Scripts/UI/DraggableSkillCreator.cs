using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DraggableSkillCreator : MonoBehaviour
{
    public Canvas createdCanvas;
    public DraggableSkill draggableSkill;
    private Image img;
    private DraggableSkill createdDraggableSkill;
    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        img = GetComponent<Image>();

        createdDraggableSkill = Instantiate(draggableSkill);
        StartCoroutine(CoWaitOneFrame());
    }

    // Grid Layout Group의 위치를 가져오기 위해 1프레임 쉬기
    private IEnumerator CoWaitOneFrame()
    {
        yield return null;

        var gridLayoutGroup = GetComponentInParent<GridLayoutGroup>();
        if (gridLayoutGroup != null)
        {
            createdDraggableSkill.transform.SetParent(createdCanvas.transform);
            createdDraggableSkill.GetRectTransform().sizeDelta = gridLayoutGroup.cellSize;
            createdDraggableSkill.GetRectTransform().position = rectTransform.position;
        }

        var canvas = GetComponentInParent<Canvas>();
        createdDraggableSkill.SetCanvas(canvas);

        img.sprite = createdDraggableSkill.GetImage().sprite;
    }
}
