using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinCon : MonoBehaviour
{
    //This is a temporary victory condition script.
    [SerializeField] string directory = "prefabs/UI/PanelWin";
    GameObject panel;
    GameObject canvas;
    // Start is called before the first frame update
    void Start()
    {
        panel = Resources.Load<GameObject>(directory);
        canvas = GameObject.Find("Canvas");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        Instantiate(panel, canvas.transform);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
