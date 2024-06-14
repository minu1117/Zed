using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    public string address;
    public bool isEnabled;
    public bool shake;
    public bool bounce;
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

    public TextMeshProUGUI dialogueTMP;
    public TextMeshProUGUI nameTMP;
    public GameObject dialoguePanel;
    public float blinkTime;
    private WaitForSeconds blinkWait;
    
    private string eventNameColumnStr;
    private string characterNameColumnStr;
    private string textColumnStr;
    private string addressColumnStr;
    private string enabledColumnStr;
    private string shakeColumnStr;
    private string bounceColumnStr;

    private string endStr;
    private string lineSplitStr;
    private string emptySellStr;
    private string addressWhiteSpaceStr;
    private string addressNormalImageStr;
    private Dictionary<string, Sprite> currentTalkImages;

    public bool isTalking = false;
    private TypingType currentTypingType;

    public CharacterImageController leftCharacterImage; // Zed
    public CharacterImageController rightCharacterImage;

    private List<string> zedNameStrList;
    private CharacterMoveController character;

    protected override void Awake()
    {
        base.Awake();

        eventNameColumnStr = "EventName";
        characterNameColumnStr = "CharacterName";
        textColumnStr = "Text";
        addressColumnStr = "Address";
        endStr = "End";
        lineSplitStr = "\\n";
        emptySellStr = "-";
        addressWhiteSpaceStr = "_";
        addressNormalImageStr = "normal";
        enabledColumnStr = "IsEnabled";
        shakeColumnStr = "Shake";
        bounceColumnStr = "Bounce";

        currentTalkImages = new Dictionary<string, Sprite>();
        blinkWait = new WaitForSeconds(blinkTime);

        zedNameStrList = new List<string>(){"제드", "우산", "고보스"};
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
                    talk = textData[textColumnStr].ToString(),
                    address = textData[addressColumnStr].ToString(),
                    isEnabled = textData[enabledColumnStr].ToString() != string.Empty ? Convert.ToBoolean(textData[enabledColumnStr]) : true,
                    shake = textData[shakeColumnStr].ToString() != string.Empty ? Convert.ToBoolean(textData[shakeColumnStr]) : false,
                    bounce = textData[bounceColumnStr].ToString() != string.Empty ? Convert.ToBoolean(textData[bounceColumnStr]) : false
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
    }

    private void ChangeActiveDialoguePanel()
    {
        dialoguePanel.SetActive(!dialoguePanel.activeSelf);
    }

    private TalkData FindFirstMatchingElement(List<TalkData> dataList, List<string> strList)
    {
        return dataList.FirstOrDefault(element => strList.Contains(element.name));
    }

    private TalkData FindFirstNotMatchingElement(List<TalkData> dataList, List<string> strList)
    {
        return dataList.FirstOrDefault(element => !strList.Contains(element.name));
    }

    private string SetDefalutAddress(string address)
    {
        if (address == null || address == emptySellStr || address == string.Empty)
            return string.Empty;

        string[] parts = address.Split(addressWhiteSpaceStr);
        return parts[0];
    }

    private async Task ApplyImage(TalkData data, Image image)
    {
        if (data.name == null || data.name == string.Empty || data.name == emptySellStr || !data.isEnabled)
        {
            image.gameObject.SetActive(false);
            return;
        }
        else
        {
            await AddressableManager.Instance.ApplyImage(data.address, image);
        }
    }

    private bool IsValidName(string name)
    {
        return name != null && name != emptySellStr && name != string.Empty;
    }

    // 현재 대화에 사용할 데이터들 미리 담아두기
    public async void SetCurrentTalk(string eventName)
    {
        if (!allTalkData.ContainsKey(eventName) || allTalkData[eventName].Count == 0)
            return;

        leftCharacterImage.gameObject.SetActive(true);
        rightCharacterImage.gameObject.SetActive(true);

        currentTalkIndex = 0;
        currentTalkData = allTalkData[eventName];

        TalkData firstZedData = FindFirstMatchingElement(currentTalkData, zedNameStrList);
        TalkData firstOtherData = FindFirstNotMatchingElement(currentTalkData, zedNameStrList);
        string zedName = firstZedData.name;
        string otherName = firstOtherData.name;

        // 첫 시작 이미지는 일반 표정의 이미지로 변경
        if (IsValidName(zedName))
        {
            firstZedData.address = $"{SetDefalutAddress(firstZedData.address)}{addressWhiteSpaceStr}{addressNormalImageStr}";
        }
        if (IsValidName(otherName))
        {
            firstOtherData.address = $"{SetDefalutAddress(firstOtherData.address)}{addressWhiteSpaceStr}{addressNormalImageStr}";
        }

        // TalkData가 할당 되지 않았을 경우 캐릭터가 없는 것으로 판단, 이미지 오브젝트 비활성화
        leftCharacterImage.SetActive(false);
        rightCharacterImage.SetActive(false);
        await ApplyImage(firstZedData, leftCharacterImage.image);
        await ApplyImage(firstOtherData, rightCharacterImage.image);

        if (!IsValidName(zedName) && !IsValidName(otherName))
            return;

        // 사용할 이미지들의 Address 모두 담아두기
        List<string> addresses = new List<string>();
        foreach (var data in currentTalkData)
        {
            if (data.isEnabled == false || data.address == emptySellStr)
                continue;

            addresses.Add(data.address);
        }

        // Dictionary에 로딩된 Sprite들의 Address를 Key로 하고, Sprite를 같이 저장
        // Address로 저장된 Image를 찾아 사용하려는 목적
        currentTalkImages = await AddressableManager.Instance.LoadSpritesToDictionary(addresses);
    }

    public void GetMessage(TypingType type)
    {
        // 현재 대화를 불러오지 못했거나, 대화가 없다면 대화를 중지하고 return
        if (currentTalkData == null || currentTalkData.Count == 0)
        {
            isTalking = false;
            character.isMoved = true;
            return;
        }

        // 출력중인 대사 코루틴이 있을 때 출력 코루틴 중지, 전체 대사로 덮어쓴 후 return
        if (messageCoroutine != null)
        {
            StopCoroutine(messageCoroutine);
            messageCoroutine = null;
            dialogueTMP.text = currentText;
            return;
        }

        // 대화가 끝났을 경우 모두 초기화 한 후 return
        if (currentTalkData.Count == currentTalkIndex)
        {
            currentTalkIndex = 0;
            isTalking = false;
            dialogueTMP.text = string.Empty;
            dialoguePanel.SetActive(false);
            character.isMoved = true;
            return;
        }

        // 현재 대화가 시작점일 경우 대화 창 Active 변경 (비활성화 -> 활성화)
        if (currentTalkIndex == 0)
        {
            ChangeActiveDialoguePanel();
        }

        var currentDialogue = currentTalkData[currentTalkIndex];
        var message = currentDialogue.talk;
        currentText = message;
        currentTalkIndex++;

        if (currentDialogue.name != string.Empty || currentDialogue.name != emptySellStr)
            nameTMP.text = currentDialogue.name;
        else
            nameTMP.text = string.Empty;

        // 현재 대사의 캐릭터 이름 비교, zedNameStrList 안에 캐릭터 이름이 있을 때 isZed = true
        bool isZed = false;
        foreach (var zedName in zedNameStrList)
        {
            if (currentDialogue.name == zedName)
            {
                isZed = true;
                break;
            }
        }

        // true : 왼쪽 이미지(주인공 자리)의 sprite 변경, 오른쪽 이미지의 색상 변경 (그림자 진 느낌의 색상으로)
        // false : true의 반대
        if (currentDialogue.name != string.Empty && currentDialogue.name != emptySellStr)
        {
            if (isZed)
            {
                ContorollCharacterImage(leftCharacterImage, rightCharacterImage, currentDialogue);
            }
            else
            {
                ContorollCharacterImage(rightCharacterImage, leftCharacterImage, currentDialogue);
            }
        }

        messageCoroutine = StartCoroutine(CoDoText(dialogueTMP, message, type));
    }

    private void ContorollCharacterImage(CharacterImageController image, CharacterImageController counterpartImage, TalkData currentDialogue)
    {
        image.SetImage(FindImage(currentDialogue.address));
        image.AdjustImageColor(counterpartImage.image);
        image.SetActive(currentDialogue.isEnabled);

        if (currentDialogue.shake)
            image.Shake();

        if (currentDialogue.bounce)
            image.JumpVertically();
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

    private Sprite FindImage(string address)
    {
        if (currentTalkImages == null || currentTalkImages.Count == 0 || address == emptySellStr)
            return null;

        return currentTalkImages[address];
    }

    public void SetCharacter(CharacterMoveController character)
    {
        this.character = character;
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
