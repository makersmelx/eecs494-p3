using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToastManager : MonoBehaviour
{
    Subscription<MessageSendEvent> sendSub;
    Subscription<MessageRemoveEvent> removeSub;

    TextMeshProUGUI toastText;
    RectTransform rect;
    CanvasGroup canvasGroup;

    string current_sender = "";

    [Header("Process Control")]
    private bool isShowing = false;
    [SerializeField] float fadeInTime = 0.5f;
    [SerializeField] float fadeOutTime = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        sendSub = EventBus.Subscribe<MessageSendEvent>(_OnNewMessage);
        removeSub = EventBus.Subscribe<MessageRemoveEvent>(_OnMessageRemoval);
        toastText = transform.Find("ToastText").GetComponent<TextMeshProUGUI>();
        canvasGroup = GetComponent<CanvasGroup>();
        // Debug.Log(canvasGroup != null);
        rect = gameObject.GetComponent<RectTransform>();
    }

    void _OnNewMessage(MessageSendEvent m)
    {
        if (isShowing) return;
        isShowing = true;
        current_sender = m.sender;
        StartCoroutine(Appear(m.message));
    }

    IEnumerator Appear(string message)
    {
        toastText.text = message;
        float initial_time = Time.time;
        float progress = (Time.time - initial_time) / fadeInTime;
        while (progress <= 1.0f)
        {
            progress = (Time.time - initial_time) / fadeInTime;
            canvasGroup.alpha = progress;
            yield return null;
        }
    }

    void _OnMessageRemoval(MessageRemoveEvent m)
    {
        if (!isShowing) return;
        if (m.sender != current_sender) return;
        StartCoroutine(Disappear());

    }

    IEnumerator Disappear()
    {
        float initial_time = Time.time;
        float progress = (Time.time - initial_time) / fadeOutTime;
        while (progress <= 1.0f)
        {
            progress = (Time.time - initial_time) / fadeOutTime;
            canvasGroup.alpha = (1f - progress);

            yield return null;
        }
        isShowing = false;
    }

}

public class MessageSendEvent
{
    public string message;
    public string sender;
    public MessageSendEvent(string message_in, string sender_in)
    {
        message = message_in;
        sender = sender_in;
    }
}

public class MessageRemoveEvent
{
    public string sender;
    public MessageRemoveEvent(string sender_in)
    {
        sender = sender_in;
    }
}

