using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetText : MonoBehaviour
{
    // Start is called before the first frame update
    Subscription<MessageSendEvent> messageSub;
    Text text;
    // [SerializeField] typeOfMessage hearing = typeOfMessage.Dialogue;
    public bool isGetText = true;
    public bool isTypewriter = false;
    public float typeSpeed = 0.05f;
    bool isTyping = false;

    void Start()
    {
        messageSub = EventBus.Subscribe<MessageSendEvent>(_OnMessage);
        text = GetComponent<Text>();
    }

    void _OnMessage(MessageSendEvent m)
    {
        text.text = "";
        if (!isTypewriter)
        {
            text.text = m.message;
        }
        else
        {
            if (isTyping) StopAllCoroutines();
            StartCoroutine(typewriting(m.message));
        }
    }
    
    IEnumerator typewriting(string input)
    {
        isTyping = true;
        foreach (char c in input)
        {
            text.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
        isTyping = false;
    }
}

public enum typeOfMessage
{
    Name,
    Dialogue
}

