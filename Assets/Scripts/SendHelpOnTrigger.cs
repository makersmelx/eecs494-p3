using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendHelpOnTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] bool isLoadFromFile = false;
    [SerializeField] string message = "";
    [SerializeField] string directory = "";

    void Start()
    {
        if (isLoadFromFile)
        {
            message = Resources.Load<TextAsset>(directory).text;
        }
    }

    //Pop message to toast when around. 
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        EventBus.Publish<MessageSendEvent>(new MessageSendEvent(message, gameObject.name));
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        EventBus.Publish<MessageRemoveEvent>(new MessageRemoveEvent(gameObject.name));

    }
}
