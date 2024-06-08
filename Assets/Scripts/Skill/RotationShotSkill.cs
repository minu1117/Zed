using DG.Tweening;
using UnityEngine;

public enum RotateType
{
    None = -1,

    X,
    Y,
    Z,

    Count,
}

public class RotationShotSkill : ShotSkill
{
    public GameObject rotateObject;
    public float rotateSpeed;

    public override void Use(GameObject charactor)
    {
        if (!isComplated || isCoolTime)
            return;

        base.Use(charactor);
        Rotate(RotateType.Y);
    }

    private void Rotate(RotateType rotateType)
    {
        Vector3 rotateVec = Vector3.zero;
        float rotate = 360 * rotateSpeed;
        switch (rotateType)
        {
            case RotateType.X:
                rotateVec.x = rotate;
                break;
            case RotateType.Y:
                rotateVec.y = rotate;
                break;
            case RotateType.Z:
                rotateVec.z = rotate;
                break;
            default:
                break;
        }

        rotateObject.transform.DOLocalRotate(rotateVec, data.duration, RotateMode.FastBeyond360)
                              .SetEase(Ease.Linear);
    }
}
