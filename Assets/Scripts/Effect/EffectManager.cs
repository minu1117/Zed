using System.Collections.Generic;

public class EffectManager : Singleton<EffectManager>
{
    public List<EffectPool> effectPools;
    private Dictionary<string, EffectPool> effectPoolDict;

    protected override void Awake()
    {
        base.Awake();

        effectPoolDict = new();
        foreach (var pool in effectPools)
        {
            var effectPool = Instantiate(pool);
            effectPool.transform.SetParent(gameObject.transform);
            effectPoolDict.Add(effectPool.GetEffectName(), effectPool);
        }
    }

    public Effect GetEffect(string effectName)
    {
        if (effectPoolDict == null || effectPoolDict.Count == 0)
            return null;

        var eft = effectPoolDict[effectName].Get();
        eft.ResetParticle();
        return eft;
    }

    public void ReleaseEffect(Effect eft)
    {
        string effectName = eft.name;
        if (effectName.EndsWith("(Clone)"))
        {
            effectName = effectName.Replace("(Clone)", "").Trim();
        }

        var pool = effectPoolDict[effectName].GetPool();
        pool.Release(eft);
    }
}
