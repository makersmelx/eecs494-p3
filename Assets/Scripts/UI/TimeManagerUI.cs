using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TimeManagerUI : MonoBehaviour
{
    [Header("UI Element Locations")]

    [Header("Value Changing")]
    [Tooltip("Wait Time before the value change text fade out")]
    public float arbitraryTimeChangeWaitTime = 8f;

    public Text timeText;
    public Text timeModifyText;

    //Reset Modify Text;
    private IEnumerator currentModifyTimer;
    // Start is called before the first frame update
    void Start()
    {


        TimeManager.Instance.TimeUpEffect += ChangeMainText;
        TimeManager.Instance.TimeFlowEffect += ChangeMainText;

        TimeManager.Instance.TimeChangeEffect += ChangeModifyText;

        TimeManager.Instance.TimeUpEffect += ResetModifyText;
        currentModifyTimer = GradualResetModifyText();

    }

    

    void ChangeMainText()
    {
        timeText.text = "Time: " + TimeManager.Instance.GetCurrentTime().ToString("N2");
    }

    void ChangeModifyText(float value)
    {
        char sign = value >= 0 ? '+' : '-';
        Color color = value >= 0 ? Color.green : Color.red;

        timeModifyText.text = sign + Mathf.Abs(value).ToString("N2") + "s";
        timeModifyText.color = color;
        StopCoroutine(currentModifyTimer);
        currentModifyTimer = GradualResetModifyText();
        StartCoroutine(currentModifyTimer);
    }

    void ResetModifyText()
    {
        timeModifyText.text = "";
    }
    IEnumerator GradualResetModifyText()
    {
        yield return new WaitForSeconds(arbitraryTimeChangeWaitTime);
        timeModifyText.text = "";
    }
}
