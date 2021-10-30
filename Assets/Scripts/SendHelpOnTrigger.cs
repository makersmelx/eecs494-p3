using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendHelpOnTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] bool is_load_from_file = false;
    [SerializeField] string message = "";
    [SerializeField] string directory = "";

    void Start()
    {
        if (is_load_from_file)
        {
            message = Resources.Load<TextAsset>(directory).text;
        }
    }

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
