using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    public List<AudioClip> sounds;
    private Dictionary<string, AudioSource> createdClips = new();

    protected override void Awake()
    {
        base.Awake();
        foreach (var clip in sounds)
        {
            GameObject audioObject = new GameObject(clip.name);
            AudioSource audioSource = audioObject.AddComponent<AudioSource>();
            audioSource.clip = clip;

            audioSource.gameObject.transform.SetParent(transform, false);
            createdClips.Add(clip.name, audioSource);
        }
    }

    public void PlayOneShot(AudioClip clip)
    {
        if (!Exist(clip.name))
            return;

        var audio = createdClips[clip.name];
        audio.PlayOneShot(audio.clip);
    }

    public void Play(AudioClip clip)
    {
        if (!Exist(clip.name))
            return;

        createdClips[clip.name].Play();
    }

    public void Stop(AudioClip clip)
    {
        if (!Exist(clip.name))
            return;

        createdClips[clip.name].Stop();
    }

    public void Pause(AudioClip clip)
    {
        if (!Exist(clip.name))
            return;

        createdClips[clip.name].Pause();
    }

    public void SetLoop(AudioClip clip, bool set)
    {
        if (!Exist(clip.name))
            return;

        createdClips[clip.name].loop = set;
    }

    private bool Exist(string name)
    {
        if (createdClips == null || createdClips.Count == 0)
            return false;

        if (createdClips[name] == null)
            return false;

        return true;
    }
}
