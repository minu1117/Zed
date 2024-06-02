using UnityEngine;

public class DialogueObject : MonoBehaviour
{
    public string eventName;
    public TypingType typingType;
    private DialogueManager manager;

    private void Awake()
    {
        manager = DialogueManager.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Zed zed))
        {
            zed.StopMove();
            manager.isTalking = true;
            manager.SetTypingType(typingType);
            manager.SetCurrentTalk(eventName);
            manager.GetMessage(typingType);
        }
    }
}
