using UnityEngine;

public class FallowTarget : MonoBehaviour
{
    public GameObject target;

    private void Update()
    {
        gameObject.transform.position = target.transform.position;
    }
}
