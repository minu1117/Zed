using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class UIRaycaster
{
    public static List<RaycastResult> GetHit(Canvas canvas)
    {
        var evSystem = canvas.GetComponent<EventSystem>();
        var raycaster = canvas.GetComponent<GraphicRaycaster>();

        PointerEventData pointerEventData = new PointerEventData(evSystem);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);

        return results;
    }

    // false == not hit
    public static bool IsHit(Canvas canvas)
    {
        return GetHit(canvas).Count > 0;
    }
}
