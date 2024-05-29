using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum TypingType
{
    None = -1,

    TypingSpeed,
    RevealTime,

    Count,
}

public struct TalkData
{
    public string name;
    public string talk;
}

public class DialogueManager : MonoBehaviour
{
    public Dictionary<string, List<TalkData>> allTalkData;
    private Coroutine messageCoroutine;
    private List<TalkData> currentTalkData;
    private string currentText;
    private int currentTalkIndex;

    public float typingSpeed;
    public float textRevealTime;

    public string csvFileName;

    public TextMeshProUGUI tempTMP;

    public void Awake()
    {
        ReadAllText();
    }

    private void ReadAllText()
    {
        List<Dictionary<string, object>> csv = CSVReader.Read(csvFileName);
        allTalkData = new Dictionary<string, List<TalkData>>();

        string currentEventName = string.Empty;
        foreach (var textData in csv)
        {
            string eventName = textData["EventName"].ToString();
            if (eventName == "End")
                continue;

            if (!string.IsNullOrEmpty(eventName))
            {
                if (!allTalkData.ContainsKey(eventName) && eventName != "-")
                {
                    currentEventName = eventName;
                    allTalkData.Add(currentEventName, new List<TalkData>());
                }

                var data = new TalkData
                {
                    name = textData["CharacterName"].ToString(),
                    talk = textData["Text"].ToString()
                };

                if (data.talk.Contains("\\n"))
                {
                    var newLineStr = data.talk.Split("\\n");
                    data.talk = string.Empty;
                    foreach (var str in newLineStr)
                    {
                        data.talk += $"{str}{Environment.NewLine}";
                    }
                }

                allTalkData[currentEventName].Add(data);
            }
        }

        //////
        //foreach (var item in allTalkData)
        //{
        //    foreach (var value in item.Value)
        //    {
        //        tempTMP.text += $"{value.talk}{Environment.NewLine}";
        //    }
        //}
    }

    public void SetCurrentTalk(string eventName)
    {
        if (!allTalkData.ContainsKey(eventName) || allTalkData[eventName].Count == 0)
            return;

        currentTalkData = allTalkData[eventName];
        currentTalkIndex = 0;
    }

    public void GetMessage(TextMeshProUGUI text, TypingType type)
    {
        if (currentTalkData == null || currentTalkData.Count == 0)
            return;

        if (messageCoroutine != null)
        {
            StopCoroutine(messageCoroutine);
            messageCoroutine = null;
            text.text = currentText;
            currentTalkIndex = 0;
            return;
        }

        var message = currentTalkData[currentTalkIndex].talk;
        currentText = message;
        currentTalkIndex++;

        messageCoroutine = StartCoroutine(CoDoText(text, message, type));
    }

    private IEnumerator CoDoText(TextMeshProUGUI text, string endValue, TypingType type)
    {
        string tempString = string.Empty;
        WaitForSeconds timer = null;
        switch (type)
        {
            case TypingType.TypingSpeed:
                timer = new WaitForSeconds(textRevealTime / endValue.Length);
                break;
            case TypingType.RevealTime:
                timer = new WaitForSeconds(typingSpeed);
                break;
            default:
                break;
        }

        if (timer == null)
            yield break;

        text.text = string.Empty;
        for (int i = 0; i < endValue.Length; i++)
        {
            tempString += endValue[i];
            text.text = tempString;

            yield return timer;
        }

        messageCoroutine = null;
    }
}
