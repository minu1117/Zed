using UnityEngine;

public class SkillWindowController : MonoBehaviour
{
    public GameObject skillWindow;

    public void ClickContorllButton()
    {
        skillWindow.SetActive(!skillWindow.activeSelf);
    }
}
