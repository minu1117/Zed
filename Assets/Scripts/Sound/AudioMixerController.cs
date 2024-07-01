using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

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
