using UnityEngine;

public class NoneTargetAutoAttack : AutoAttack
{
    public override void Attack(GameObject character)
    {
        Vector3 point = Raycast.GetMousePointVec();
        point.y = character.transform.position.y;
        character.transform.LookAt(point);
        //StartUseSound();
    }
}
