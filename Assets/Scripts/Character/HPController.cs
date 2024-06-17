using System;
using UnityEngine;
using UnityEngine.UI;

public class HPController : MonoBehaviour
{
    public Slider slider;
    public Canvas canvas;
    private CharacterData data;
    private Camera cam;

    private void Awake()
    {
        data = GetComponent<ChampBase>().data;
        cam = Camera.main;
        canvas.worldCamera = cam;
        data.currentHp = data.maxhp;

        slider.gameObject.transform.position = gameObject.transform.position + Vector3.up * (gameObject.GetComponent<Collider>().bounds.size.y);
        SetCurrentHP(data.currentHp);
    }

    public void SetCurrentHP(float currentHp)
    {
        data.currentHp = Math.Clamp(currentHp, 0, data.currentHp);
        slider.value = data.currentHp / data.maxhp;
    }

    public void SetMaxHP()
    {
        data.currentHp = data.maxhp;
        SetCurrentHP(data.maxhp);
    }

    public void Update()
    {
        slider.gameObject.transform.rotation = cam.transform.rotation;
    }
}
