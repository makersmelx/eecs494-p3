using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetText : MonoBehaviour
{
    // Start is called before the first frame update
    Subscription<MessageSendEvent> message_sub;
    Text text;
    [SerializeField] typeOfMessage hearing = typeOfMessage.Dialogue;
    public bool is_get_text = true;
    public bool is_typewriter = false;
    public float type_speed = 0.05f;
    bool is_typing = false;
    void Start()
    {
        message_sub = EventBus.Subscribe<MessageSendEvent>(_OnMessage);
        text = GetComponent<Text>();
    }

    void _OnMessage(MessageSendEvent m)
    {
        text.text = "";
        if (!is_typewriter)
        {
            text.text = m.message;
        }
        else
        {
            if (is_typing) StopAllCoroutines();
            StartCoroutine(typewriting(m.message));
        }
    }
    
    IEnumerator typewriting(string input)
    {
        is_typing = true;
        foreach (char c in input)
        {
            text.text += c;
            yield return new WaitForSeconds(type_speed);
        }
        is_typing = false;
    }
}

public enum typeOfMessage
{
    Name,
    Dialogue
}



