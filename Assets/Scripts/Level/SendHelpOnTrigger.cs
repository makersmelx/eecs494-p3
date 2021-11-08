using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendHelpOnTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] bool isLoadFromFile = false;
    [TextArea] public string message = "";
    [SerializeField] string directory = "";

    private bool active = false;

    void Start()
    {
        if (isLoadFromFile)
        {
            message = Resources.Load<TextAsset>(directory).text;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (active) return;

        if (!other.gameObject.CompareTag("Player")) return;

        EventBus.Publish<MessageSendEvent>(new MessageSendEvent(message, gameObject.name));
        active = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        EventBus.Publish<MessageRemoveEvent>(new MessageRemoveEvent(gameObject.name));
        active = false;
    }
}
