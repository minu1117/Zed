using UnityEngine;
using UnityEngine.EventSystems;

public class SortingCanvasChilden : MonoBehaviour, IPointerClickHandler
{
    private CanvasSoringOrderController controller;
    private Canvas canvas;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        controller = GetComponentInParent<CanvasSoringOrderController>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        controller.SetMaxOrder(this);
    }

    public void SetSortingOrder(int order)
    {
        canvas.sortingOrder = order;
    }
}
