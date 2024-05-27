using UnityEngine;

public class FallowTarget : MonoBehaviour
{
    public GameObject target;

    private void FixedUpdate()
    {
        if (target == null) return;

        gameObject.transform.position = target.transform.position;
    }
}
