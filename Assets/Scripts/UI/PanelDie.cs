using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelDie : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnEnable()
    {
        Cursor.visible=true;
        Cursor.lockState = CursorLockMode.None;

    }

    // Update is called once per frame
    public void OnClick()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        TimeManager.Instance.ResetTimer();
        gameObject.SetActive(false);
    }
}
