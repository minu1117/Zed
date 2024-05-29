using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    [Tooltip("임시 메세지 키, 밸류 작성 (페어 맞게)")]
    public List<string> tempKeyStrings;
    public List<string> tempValueStrings1;
    public List<string> tempValueStrings2;
    public List<string> tempValueStrings3;
    private Dictionary<string, List<string>> messages;

    public TextMeshProUGUI text;
    private Coroutine messageCoroutine;
    private string currentText;

    public float duration;

    public void Awake()
    {
        //text.text = string.Empty;
        //messages = new Dictionary<string, List<string>>();

        //// 임시 작성
        //for (int i = 0; i < tempKeyStrings.Count; i++)
        //{
        //    List<string> strings = null;
        //    if (i == 0)
        //        strings = tempValueStrings1;
        //    else if (i == 1)
        //        strings = tempValueStrings2;
        //    else
        //        strings = tempValueStrings3;

        //    if (strings == null)
        //        continue;

        //    messages.Add(tempKeyStrings[i], strings);
        //}
    }

    //public void Update()
    //{
    //    if (Input.GetKeyUp(KeyCode.Alpha1)) 
    //    {
    //        GetMessage(tempKeyStrings[0], duration);
    //    }
    //    if (Input.GetKeyUp(KeyCode.Alpha2))
    //    {
    //        GetMessage(tempKeyStrings[1], duration);
    //    }
    //    if (Input.GetKeyUp(KeyCode.Alpha3))
    //    {
    //        GetMessage(tempKeyStrings[2], duration);
    //    }
    //}

    //public void GetMessage(string key, float duration)
    //{
    //    if (!messages.ContainsKey(key) || messages[key].Count == 0)
    //        return;

    //    if (messageCoroutine != null)
    //    {
    //        StopCoroutine(messageCoroutine);
    //        messageCoroutine = null;
    //        text.text = currentText;
    //        return;
    //    }

    //    var message = messages[key];

    //    var newText = message.First();
    //    messages[key].RemoveAt(0);
    //    messages[key].Add(newText);
    //    currentText = newText;
    //    messageCoroutine = StartCoroutine(DoText(text, newText, duration));
    //}

    //private IEnumerator DoText(TextMeshProUGUI text, string endValue, float duration)
    //{
    //    string tempString = string.Empty;
    //    WaitForSeconds charPerTime = new WaitForSeconds(duration / endValue.Length);

    //    for (int i = 0; i < endValue.Length; i++)
    //    {
    //        tempString += endValue[i];
    //        text.text = tempString;

    //        yield return charPerTime;
    //    }

    //    messageCoroutine = null;
    //}
}
