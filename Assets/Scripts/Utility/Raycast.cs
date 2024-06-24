using UnityEngine;

public static class Raycast
{
    public static Vector3 GetMousePointVec()
    {
        Vector3 point = Input.mousePosition;
        string layerMaskName = "Plane";
        var hit = GetHit(point, layerMaskName);

        if (hit.collider == null)
            return Vector3.zero;

        return hit.point;
    }

    public static RaycastHit GetHit(Vector3 point, string layerMask)
    {
        var ray = Camera.main.ScreenPointToRay(point);
        RaycastHit hit;
        int mask = LayerMask.GetMask(layerMask);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
        {
            return hit;
        }

        return default;
    }

    public static (GameObject, bool) FindMousePosTarget(string layerMask)
    {
        var hit = GetHit(Input.mousePosition, layerMask);
        if (hit.collider != null)
        {
            return (hit.collider.gameObject, true);
        }

        return (null, false);
    }
}
