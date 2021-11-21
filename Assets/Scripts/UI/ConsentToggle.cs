using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsentToggle : MonoBehaviour
{
    public Toggle toggle;

    private void Start()
    {
        toggle = GetComponent<Toggle>();
        toggle.isOn = CustomAnalyticsEvent.willEnableAnalytic;
    }

    public void OnValueChange(bool change)
    {
        CustomAnalyticsEvent.willEnableAnalytic = change;
    }
}