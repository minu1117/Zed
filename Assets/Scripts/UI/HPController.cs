using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum CanvasMode
{
    WorldSpace,
    Overlay,
}

public enum SliderMode
{
    HP,
    MP,
}

public class HPController : MonoBehaviour
{
    public Slider slider;
    public Canvas canvas;
    public ChampBase champ;
    public TextMeshProUGUI tmp;
    public CanvasMode canvasMode;
    public SliderMode sliderMode;
    private CharacterData data;
    private Camera cam;

    private void Awake()
    {
        if (champ == null)
            champ = Zed.Instance;

        data = champ.data;
        ChangeCanvasMode(canvasMode);
        data.currentHp = data.maxHp;
        data.currentMp = data.maxMp;

        SetMaxValue();
    }

    public void SetCurrentValue()
    {
        switch (sliderMode)
        {
            case SliderMode.HP:
                SetCurrentHp();
                break;
            case SliderMode.MP:
                SetCurrentMp();
                break;
        }
    }

    public void SetMaxValue()
    {
        switch (sliderMode)
        {
            case SliderMode.HP:
                SetMaxHp();
                break;
            case SliderMode.MP:
                SetMaxMp();
                break;
        }
    }

    public void SetCurrentHp()
    {
        data.currentHp = Math.Clamp(data.currentHp, 0, data.maxHp);
        slider.value = data.currentHp / data.maxHp;
        SetText($"{data.currentHp} / {data.maxHp}");
    }

    public void SetMaxHp()
    {
        data.currentHp = data.maxHp;
        SetCurrentHp();
        SetText($"{data.currentHp} / {data.maxHp}");
    }

    public void SetCurrentMp()
    {
        data.currentMp = Math.Clamp(data.currentMp, 0, data.maxMp);
        slider.value = data.currentMp / data.maxMp;
        SetText($"{data.currentMp} / {data.maxMp}");
    }

    public void SetMaxMp()
    {
        data.currentMp = data.maxMp;
        SetCurrentMp();
        SetText($"{data.currentMp} / {data.maxMp}");
    }

    private void SetText(string text)
    {
        tmp.text = text;
    }

    public void Update()
    {
        LookCamera();
    }

    private void LookCamera()
    {
        if (canvasMode == CanvasMode.WorldSpace && cam != null)
        {
            slider.gameObject.transform.rotation = cam.transform.rotation;
        }
    }

    private void ChangeCanvasMode(CanvasMode mode)
    {
        switch (mode)
        {
            case CanvasMode.WorldSpace:
                cam = Camera.main;
                canvas.renderMode = RenderMode.WorldSpace;
                canvas.worldCamera = cam;
                slider.gameObject.transform.position = gameObject.transform.position + Vector3.up * (gameObject.GetComponent<Collider>().bounds.size.y);
                break;
            case CanvasMode.Overlay:
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                break;
        }
    }
}
