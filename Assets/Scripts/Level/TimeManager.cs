using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    public delegate void NormalUpdate();

    //Register everything that is done at TimeUp, TimeFlow and TimeReset. 
    public NormalUpdate TimeUpEffect;
    public NormalUpdate TimeFlowEffect;

    //Register everything that is done when we arbitrarily changes the time. 
    public delegate void ChangeTimeCallback(float previous, float current, float change);
    private List<ChangeTimeCallback> callbacks = new List<ChangeTimeCallback>();

    [Header("Time Manager Settings")]
    [Tooltip("Time limit that this level provides")]
    public float maxTime = 20f;

    // -------------------------------------------------------------------------
    // Internal State
    // -------------------------------------------------------------------------
    private float timeRemaining;

    ////Use to reduce update rate and make your eyes feel better. 
    //private int alternator = 0;
    //private int updatePeriod = 5;


    // -------------------------------------------------------------------------
    // Singleton
    // -------------------------------------------------------------------------
    public static TimeManager Instance;

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


    // -------------------------------------------------------------------------
    // Public methods
    // -------------------------------------------------------------------------
    public void AddTime(float value)
    {
        ChangeTime(value);
    }

    public void ReduceTime(float value)
    {
        ChangeTime(-value);
    }

    public void ResetTimer()
    {
        timeRemaining = maxTime;
    }

    public float GetCurrentTime()
    {
        return timeRemaining;
    }

    public void AddTimeChangeCallback(ChangeTimeCallback func)
    {
        callbacks.Add(func);
    }

    // -------------------------------------------------------------------------
    // Private methods
    // -------------------------------------------------------------------------
    private void Start()
    {
        timeRemaining = maxTime;
    }

    void Update()
    {
        MockTimeChange();
        timeRemaining -= Time.deltaTime;

        if (timeRemaining < 0.01f)
        {
            timeRemaining = 0;
            TimeUpEffect();
        }
    }

    private void ChangeTime(float value)
    {
        Debug.Log("Time change:" + value.ToString());
        float newTimeRemaining = Mathf.Clamp(timeRemaining + value, 0, maxTime);

        foreach (ChangeTimeCallback callback in callbacks)
        {
            callback(timeRemaining, newTimeRemaining, value);
        }

        timeRemaining = newTimeRemaining;
    }

    private void MockTimeChange()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ReduceTime(1f);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            AddTime(1f);
        }
    }
}