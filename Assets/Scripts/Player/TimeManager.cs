using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    public delegate void NormalUpdate();
    //Register everything that is done at TimeUp, TimeFlow and TimeReset. 
    public NormalUpdate TimeUpEffect;
    public NormalUpdate TimeFlowEffect;

    //Register everything that is done when we arbitrarily changes the time. 
    public delegate void ModifyUpdate(float value);
    public ModifyUpdate TimeChangeEffect;

    

    [Header("Initial Settings")]
    [Tooltip("Time Limit that this level provides")]
    public float timeLimit;

    private float currentTime;

    //Use to reduce update rate and make your eyes feel better. 
    private int alternator = 0;
    private int updatePeriod = 5;




    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        currentTime = timeLimit;
    }
    // Update is called once per frame


    void Update()
    {
        currentTime -= Time.deltaTime;

        if (currentTime<0.01f)
        {
            currentTime = 0;
            TimeUpEffect();
        }

        if (alternator == 0)
        {
            TimeFlowEffect();
        }
        alternator += 1; 
        if (alternator >= updatePeriod)
        {
            alternator = 0;
        }
    }

    public void AddTime(float value)
    {
        ChangeTime(value);

    }

    public void ReduceTime(float value)
    {
        ChangeTime(-value);
    }

    private void ChangeTime(float value)
    {
        currentTime += value;

        TimeChangeEffect(value);
        //UI Part.
    }

    public void ResetTimer()
    {
        currentTime = timeLimit;
    }

    public float GetCurrentTime() { return currentTime; }
   

}
