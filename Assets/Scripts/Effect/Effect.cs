using System.Collections;
using UnityEngine;

public class Effect : MonoBehaviour
{
    [SerializeField] protected ParticleSystem particle;
    private Vector3 startPos;

    protected virtual void Awake()
    {
        ResetParticle();
    }

    public virtual void Use()
    {
        ResetParticle();
        particle.Play();
        StartCoroutine(CheckParticleAlive());
    }

    public virtual void Stop()
    {
        particle.Stop();
    }

    public void ResetParticle()
    {
        particle.Stop();
        particle.Clear();
    }

    public void SetStartPos(Vector3 pos)
    {
        startPos = pos;
        particle.transform.position = startPos;
    }

    private IEnumerator CheckParticleAlive()
    {
        yield return new WaitUntil(() => particle.IsAlive(true) == false);
        gameObject.SetActive(false);
    }
}
