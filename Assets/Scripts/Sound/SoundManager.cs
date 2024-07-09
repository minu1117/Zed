using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class SoundManager : Singleton<SoundManager>
{
    public List<AudioClip> bgmSounds;
    public List<AudioClip> sfxSounds;
    private Dictionary<string, AudioSource> createdClips = new();

    protected override void Awake()
    {
        base.Awake();
        CreateSound(sfxSounds, AudioType.SFX);
        CreateSound(bgmSounds, AudioType.BGM);
    }

    private void CreateSound(List<AudioClip> clips, AudioType type)
    {
        if (clips == null || clips.Count == 0)
            return;

        var parentObj = new GameObject(EnumConverter.GetString(type));
        parentObj.gameObject.transform.SetParent(transform, false);

        foreach (var clip in clips)
        {
            GameObject audioObject = new GameObject(clip.name);
            AudioSource source = audioObject.AddComponent<AudioSource>();
            source.clip = clip;
            source.playOnAwake = false;

            source.gameObject.transform.SetParent(parentObj.transform, false);
            source.outputAudioMixerGroup = AudioMixerController.Instance.GetAudioMixer(type);
            createdClips.Add(clip.name, source);
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

        if (!createdClips.ContainsKey(name))
            return false;

        return true;
    }
}
