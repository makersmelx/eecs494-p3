using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TimeManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static TimeManager instance;

    public delegate void WhenTImeIsUpDo();
    //Register everything that is done when TimeUp here. 
    public WhenTImeIsUpDo TimeUpEffect;
    [Header("Initial Settings")]
    [Tooltip("Time Limit that this level provides")]
    public float timeLimit;


    int updatePeriod = 5;
    [Header("Value Changing")]
    [Tooltip("Wait Time before the value change text fade out")]
    public float arbitraryTimeChangeWaitTime = 8f;

    private Text timeText;
    private Text timeModifyText;
    private float currentTime;
    private int alternator = 0;

    //Reset Modify Text;
    private IEnumerator currentModifyTimer;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        currentTime = timeLimit;
        timeText = transform.Find("TimeText").GetComponent<Text>();
        timeModifyText = transform.Find("TimeModifyText").GetComponent<Text>();

        currentModifyTimer = GradualResetModifyText();

        timeText.text = "Time: " + currentTime.ToString("N2");
    }
    // Update is called once per frame


    void Update()
    {
        currentTime -= Time.deltaTime;

        if (currentTime<0.01f)
        {
            currentTime = 0;
            timeText.text = "Time: " + currentTime.ToString("N2");
            TimeUpEffect();
        }

        if (alternator == 0)
        {
            timeText.text = "Time: " + currentTime.ToString("N2");
        }
        alternator += 1; 
        if (alternator >= updatePeriod)
        {
            alternator = 0;
        }
    }

    public void AddTime(float value)
    {
        ChangeTime(value, '+', Color.green);

    }

    public void ReduceTime(float value)
    {
        ChangeTime(-value, '-', Color.red);
    }

    private void ChangeTime(float value, char sign, Color color)
    {
        currentTime += value;
        timeModifyText.text = sign + Mathf.Abs(value).ToString("N2") + "s";
        Debug.Log(sign + value.ToString("N2") + "s");
        timeModifyText.color = color;
        StopCoroutine(currentModifyTimer);
        currentModifyTimer = GradualResetModifyText();
        StartCoroutine(currentModifyTimer);
    }

    public void ResetTimer()
    {
        currentTime = timeLimit;
        timeModifyText.text = "";
    }

    IEnumerator GradualResetModifyText()
    {
        yield return new WaitForSeconds(arbitraryTimeChangeWaitTime);
        timeModifyText.text = "";
    }


}
