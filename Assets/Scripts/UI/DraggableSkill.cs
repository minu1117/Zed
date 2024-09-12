using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class DraggableSkill : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerExitHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Image img;
    private SortingGroup sortingGroup;

    public ZedSkillButtonData skill;

    private int minOrder = 1;
    private int maxOrder = 999;

    private bool isDrag;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        sortingGroup = GetComponent<SortingGroup>();
        img = GetComponent<Image>();
        img.sprite = skill.sp;

        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDrag = true;
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
        sortingGroup.sortingOrder = maxOrder;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDrag = false;
        canvasGroup.alpha = 0f;
        sortingGroup.sortingOrder = minOrder;
        canvasGroup.blocksRaycasts = true;
        gameObject.SetActive(false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isDrag)
            return;

        isDrag = false;
        canvasGroup.alpha = 0f;
        sortingGroup.sortingOrder = minOrder;
        canvasGroup.blocksRaycasts = true;
        gameObject.SetActive(false);
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
