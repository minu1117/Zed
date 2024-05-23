using UnityEngine;

public static class Raycast
{
    public static Vector3 GetMousePointVec()
    {
        //var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //RaycastHit hit;
        //int layerMask = LayerMask.GetMask("Plane");

        //if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        //{
        //    return hit.point;
        //}

        Vector3 point = Input.mousePosition;
        string layerMaskName = "Plane";
        var hit = GetHit(point, layerMaskName);

        if (hit.collider == null)
            return Vector3.zero;

        return hit.point;
    }

    public static RaycastHit GetHit(Vector3 point, string layerMaskName)
    {
        var ray = Camera.main.ScreenPointToRay(point);
        RaycastHit hit;
        int layerMask = LayerMask.GetMask(layerMaskName);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            return hit;
        }

        return default;
    }
}
