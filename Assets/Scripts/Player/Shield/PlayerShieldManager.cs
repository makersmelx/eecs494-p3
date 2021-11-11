using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShieldManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject shield;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        SetActive();
    }

    private void SetActive()
    {
        shield.SetActive(PlayerInputHandler.Instance.GetMouseRightButton());
    }
}