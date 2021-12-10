using System;
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

    [Header("Time Manager Settings")] [Tooltip("Time limit that this level provides")]
    public float maxTime = 20f;

    //After fastFlowThreshold * maxtime has spent in a level, the time flow would accelerate increaseFactor
    // per second (time losing becomes squared).
    public float fastFlowThreshold = 1.4f;
    public float increaseFactor = 0.5f;

    // -------------------------------------------------------------------------
    // Internal State
    // -------------------------------------------------------------------------
    private float timeRemaining;
    private float velocity = 1f;

    private float timeSpent = 0f;

    public DateTime startTime;
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
        timeSpent = 0;
        velocity = 1;
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
        startTime = DateTime.Now;
    }

    void Update()
    {
        // MockTimeChange();
        timeRemaining -= Time.deltaTime;
        timeSpent += Time.deltaTime;

        UpdateVelocity();

        if (timeRemaining < 0.001f)
        {
            timeRemaining = 0;
            TimeUpEffect();
        }
    }

    private void ChangeTime(float value)
    {
        float newTimeRemaining = Mathf.Clamp(timeRemaining + value, 0, maxTime);

        foreach (ChangeTimeCallback callback in callbacks)
        {
            callback(timeRemaining, newTimeRemaining, value);
        }

        timeRemaining = newTimeRemaining;
    }

    // Change velocity above threshold. 
    private void UpdateVelocity()
    {
        if (timeSpent >= maxTime * fastFlowThreshold)
        {
            velocity += increaseFactor * Time.deltaTime;
        }
    }

    private void MockTimeChange()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ReduceTime(10f);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            AddTime(10f);
        }
    }
}