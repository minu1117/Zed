using System.Collections.Generic;
using UnityEngine;

public class CanvasSoringOrderController : MonoBehaviour
{
    public List<SortingCanvasChilden> canvasList;
    private int minOrderIndex = 1;
    private int maxOrderIndex = 999;

    public int GetMaxOrder()
    {
        return maxOrderIndex;
    }

    public void SetMaxOrder(SortingCanvasChilden canvas)
    {
        foreach (var item in canvasList)
        {
            if (ReferenceEquals(item, canvas))
            {
                item.SetSortingOrder(maxOrderIndex);
                continue;
            }

            item.SetSortingOrder(minOrderIndex);
        }
    }
}
