using UnityEngine;
using UnityEngine.Pool;

public class EffectPool : MonoBehaviour
{
    [SerializeField] private Effect effect;
    [SerializeField] private int maxPoolSize;
    private IObjectPool<Effect> pool;

    private void Awake()
    {
        pool = new ObjectPool<Effect>
                (
                    CreateEffect,
                    GetEffect,
                    ReleaseEffect,
                    DestroyEffect,
                    maxSize: maxPoolSize
                );
    }

    public IObjectPool<Effect> GetPool()
    {
        return pool;
    }

    public string GetEffectName()
    {
        return effect.name;
    }

    public Effect Get()
    {
        if (pool == null)
            return null;

        return pool.Get();
    }

    private Effect CreateEffect()
    {
        var useEffect = Instantiate(effect, transform);
        return useEffect;
    }
    private void GetEffect(Effect eft)
    {
        eft.gameObject.SetActive(true);
    }
    private void ReleaseEffect(Effect eft)
    {
        eft.Stop();
    }
    private void DestroyEffect(Effect eft)
    {
        Destroy(eft.gameObject);
    }
}
