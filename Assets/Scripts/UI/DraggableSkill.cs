using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableSkill : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Image img;
    private Vector3 firstPos;

    public ZedSkillButtonData skill;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        img = GetComponent<Image>();
        img.sprite = skill.sp;

        canvasGroup.alpha = 0f;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        firstPos = rectTransform.anchoredPosition;
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = true;
        rectTransform.anchoredPosition = firstPos;
    }

    public Image GetImage()
    {
        return img;
    }

    public RectTransform GetRectTransform()
    {
        return rectTransform;
    }

    public void SetCanvas(Canvas canvas)
    {
        this.canvas = canvas;
    }
}
