using UnityEngine;

public class RaycastedCanvas : MonoBehaviour
{
    private Canvas canvas;

    public void Awake()
    {
        canvas = GetComponent<Canvas>();
    }

    // ray를 쏴 캔버스에 맞았는 지 확인
    public bool IsHit()
    {
        return UIRaycaster.IsHit(canvas);
    }
}
