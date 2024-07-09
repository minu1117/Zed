using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public enum AudioType
{
    Master,
    BGM,
    SFX,
}

public class AudioMixerController : Singleton<AudioMixerController>
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider MasterSlider;
    [SerializeField] private Slider BGMSlider;
    [SerializeField] private Slider SFXSlider;

    public string MasterGroupName;
    public string BGMGroupName;
    public string SFXGroupName;

    protected override void Awake()
    {
        base.Awake();
        //MasterSlider.onValueChanged.AddListener(SetMasterVolume);
        //BGMSlider.onValueChanged.AddListener(SetMusicVolume);
        //SFXSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public AudioMixerGroup GetAudioMixer(AudioType type)
    {
        AudioMixerGroup[] audioMixerGroups = mixer.FindMatchingGroups(EnumConverter.GetString(type));
        if (audioMixerGroups.Length > 0)
        {
            return audioMixerGroups[0];
        }
        else
        {
            Debug.LogError("SFX 그룹을 찾을 수 없습니다.");
            return null;
        }
    }

    public void SetMasterVolume(float volume)
    {
        mixer.SetFloat(MasterGroupName, Mathf.Log10(volume) * 20);
    }

    public void SetMusicVolume(float volume)
    {
        mixer.SetFloat(BGMGroupName, Mathf.Log10(volume) * 20);
    }

    public void SetSFXVolume(float volume)
    {
        mixer.SetFloat(SFXGroupName, Mathf.Log10(volume) * 20);
    }
}
