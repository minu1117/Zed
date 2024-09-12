using UnityEngine;

public class RaycastedCanvas : MonoBehaviour
{
    private Canvas canvas;

    public void Awake()
    {
        canvas = GetComponent<Canvas>();
    }

    public bool IsHit()
    {
        return UIRaycaster.IsHit(canvas);
    }
}
