using UnityEngine;

public class TargetFollowEffect : Effect
{
    private GameObject target;

    public override void Use()
    {
        base.Use();
    }

    public void SetTarget(GameObject obj)
    {
        target = obj;
    }

    public override void Stop()
    {
        base.Stop();
        target = null;
    }

    private void FollowTargetPos()
    {
        if (target == null || particle == null || particle.isStopped)
            return;

        particle.gameObject.transform.position = target.transform.position;
    }

    private void Update()
    {
        FollowTargetPos();
    }
}
