using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ToastManager : MonoBehaviour
{
    Subscription<MessageSendEvent> send_sub;
    Subscription<MessageRemoveEvent> remove_sub;

    Text toast_text;
    RectTransform rect;
    CanvasGroup cgroup;

    string current_sender = "";

    [Header("Process Control")]
    private bool is_showing = false;
    [SerializeField] float fade_in_time = 0.5f;
    [SerializeField] float fade_out_time = 0.5f;

    

    // Start is called before the first frame update
    void Start()
    {
        send_sub = EventBus.Subscribe<MessageSendEvent>(_OnNewMessage);
        remove_sub = EventBus.Subscribe<MessageRemoveEvent>(_OnMessageRemoval);
        toast_text = transform.Find("ToastText").GetComponent<Text>();
        cgroup = GetComponent<CanvasGroup>();
        Debug.Log(cgroup != null);
        rect = gameObject.GetComponent<RectTransform>();
    }
    void _OnNewMessage(MessageSendEvent m)
    {
        if (is_showing) return;
        is_showing = true;
        current_sender = m.sender;
        StartCoroutine(appear(m.message));
    }
    IEnumerator appear(string message)
    {
        toast_text.text = message;
        float initial_time = Time.time;
        float progress = (Time.time - initial_time) / fade_in_time;
        while (progress <= 1.0f)
        {
            progress = (Time.time - initial_time) / fade_out_time;
            cgroup.alpha = progress;
            yield return null;
        }
    }
    void _OnMessageRemoval(MessageRemoveEvent m)
    {
        Debug.Log("Received!");
        if (!is_showing) return;
        if (m.sender != current_sender) return;
        StartCoroutine(disappear());

    }
    IEnumerator disappear()
    {
        float initial_time = Time.time;
        float progress = (Time.time - initial_time) / fade_out_time;
        while (progress <= 1.0f)
        {
            progress = (Time.time - initial_time) / fade_out_time;
            cgroup.alpha = (1f-progress);
            
            yield return null;
        }
        is_showing = false;
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
    public MessageRemoveEvent( string sender_in)
    {
        sender = sender_in;
    }
}

