using UnityEngine;

public class DialogueObject : MonoBehaviour
{
    public string eventName;
    public TypingType typingType;
    private DialogueManager manager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Zed zed))
        {
            manager = DialogueManager.Instance;
            zed.StopMove();

            manager.isTalking = true;
            manager.SetTypingType(typingType);
            manager.SetCurrentTalk(eventName);
            manager.GetMessage(typingType);
        }
    }
}
