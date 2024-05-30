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

public class DialogueManager : Singleton<DialogueManager>
{
    public Dictionary<string, List<TalkData>> allTalkData;
    private Coroutine messageCoroutine;
    private List<TalkData> currentTalkData;
    private string currentText;
    private int currentTalkIndex;

    public float typingSpeed;
    public float textRevealTime;

    public string csvFileName;

    public TextMeshProUGUI tmp;
    public GameObject dialoguePanel;
    public float blinkTime;
    private WaitForSeconds blinkWait;
    
    private string eventNameColumnStr;
    private string characterNameColumnStr;
    private string textColumnStr;

    private string endStr;
    private string lineSplitStr;
    private string emptySellStr;

    public bool isTalking = false;
    private TypingType currentTypingType;

    protected override void Awake()
    {
        base.Awake();

        eventNameColumnStr = "EventName";
        characterNameColumnStr = "CharacterName";
        textColumnStr = "Text";
        endStr = "End";
        lineSplitStr = "\\n";
        emptySellStr = "-";

        blinkWait = new WaitForSeconds(blinkTime);

        ReadAllText();
    }

    private void ReadAllText()
    {
        List<Dictionary<string, object>> csv = CSVReader.Read(csvFileName);
        allTalkData = new Dictionary<string, List<TalkData>>();

        string currentEventName = string.Empty;
        foreach (var textData in csv)
        {
            string eventName = textData[eventNameColumnStr].ToString();
            if (eventName == endStr)
                continue;

            if (!string.IsNullOrEmpty(eventName))
            {
                // 겹치는 EventName 키가 없고, 이벤트 이름이 emptySellStr이 아닐 경우
                // 이벤트 별로 구분하여 대화를 Dictionary에 저장
                if (!allTalkData.ContainsKey(eventName) && eventName != emptySellStr)
                {
                    currentEventName = eventName;
                    allTalkData.Add(currentEventName, new List<TalkData>());
                }

                var data = new TalkData
                {
                    name = textData[characterNameColumnStr].ToString(),
                    talk = textData[textColumnStr].ToString()
                };

                // 저장한 대화에서 개행 처리를 해야 할 경우
                if (data.talk.Contains(lineSplitStr))
                {
                    // Line Split 문자가 있는 구간마다 문자 분리
                    // 분리된 문자를 개행 처리 한 후 데이터에 추가
                    string[] lineSplit = data.talk.Split(lineSplitStr);
                    data.talk = string.Empty;
                    for (int i = 0; i < lineSplit.Length; i++) 
                    {
                        // 마지막 줄이 아닐 경우에만 개행
                        string splitStr = lineSplit[i];
                        if (i < lineSplit.Length - 1)
                        {
                            splitStr = $"{splitStr}{Environment.NewLine}";
                        }

                        data.talk += splitStr;
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

    private void ChangeActiveDialoguePanel()
    {
        dialoguePanel.SetActive(!dialoguePanel.activeSelf);
    }

    public void SetCurrentTalk(string eventName)
    {
        if (!allTalkData.ContainsKey(eventName) || allTalkData[eventName].Count == 0)
            return;

        currentTalkData = allTalkData[eventName];
        currentTalkIndex = 0;
    }

    public void GetMessage(TypingType type)
    {
        if (currentTalkData == null || currentTalkData.Count == 0)
            return;

        if (messageCoroutine != null)
        {
            StopCoroutine(messageCoroutine);
            messageCoroutine = null;
            tmp.text = currentText;
            return;
        }

        if (currentTalkData.Count == currentTalkIndex)
        {
            currentTalkIndex = 0;
            isTalking = false;
            tmp.text = string.Empty;
            dialoguePanel.SetActive(false);
            Zed.Instance.isMoved = true;
            return;
        }

        if (currentTalkIndex == 0)
        {
            ChangeActiveDialoguePanel();
        }

        var message = currentTalkData[currentTalkIndex].talk;
        currentText = message;
        currentTalkIndex++;

        messageCoroutine = StartCoroutine(CoDoText(tmp, message, type));
    }

    private IEnumerator CoDoText(TextMeshProUGUI text, string endValue, TypingType type)
    {
        string tempString = string.Empty;
        WaitForSeconds timer = null;

        switch (type)
        {
            case TypingType.TypingSpeed:
                timer = new WaitForSeconds(typingSpeed);
                break;
            case TypingType.RevealTime:
                timer = new WaitForSeconds(textRevealTime / endValue.Length);
                break;
            default:
                break;
        }

        if (timer == null)
            yield break;

        StartCoroutine(CoBlinkDialoguePanel());

        text.text = string.Empty;
        for (int i = 0; i < endValue.Length; i++)
        {
            tempString += endValue[i];
            text.text = tempString;

            yield return timer;
        }

        messageCoroutine = null;
    }

    private IEnumerator CoBlinkDialoguePanel()
    {
        ChangeActiveDialoguePanel();

        yield return blinkWait;

        ChangeActiveDialoguePanel();
    }

    public void SetTypingType(TypingType type)
    {
        currentTypingType = type;
    }

    public void Update()
    {
        if (isTalking)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                GetMessage(currentTypingType);
            }
        }
    }
}
