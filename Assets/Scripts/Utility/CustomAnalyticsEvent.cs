using System;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_CLOUD_SERVICES_ANALYTICS
using UnityEngine.Analytics;
#endif


public class CustomAnalyticsEvent : MonoBehaviour
{
    // -------------------------------------------------------------------------
    // Singleton
    // -------------------------------------------------------------------------
    public static CustomAnalyticsEvent instance = null;
    public static bool willEnableAnalytic = true;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    // -------------------------------------------------------------------------
    // Public methods
    // -------------------------------------------------------------------------

    public void GameStartEvent(float time)
    {
        ReportEvent("Game Start", "Time", time);
    }

    public void LevelFailedEvent(Vector3 position, float time)
    {
        ReportEvent("Level Failed", "Position", position);
        ReportEvent("Level Failed", "Time", time);
    }

    public void GameQuitEvent(float time)
    {
        ReportEvent("Game Quit", "Time", time);
    }

    public void CheckpointResetEvent(Vector3 position, float time)
    {
        ReportEvent("Checkpoint Reset", "Position", position);
        ReportEvent("Checkpoint Reset", "Time", time);
    }


    // -------------------------------------------------------------------------
    // Private methods
    // -------------------------------------------------------------------------
    private void ReportEvent(string eventName, string key, object value)
    {
#if ENABLE_CLOUD_SERVICES_ANALYTICS
        Dictionary<string, object> eventDict = new Dictionary<string, object>
        {
            {key, value}
        };
        Analytics.CustomEvent(eventName, eventDict);
#endif
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void DisableAnalyticByDefault()
    {
        // default allowing data collection
        Analytics.initializeOnStartup = false;
    }

    public void ConfigureAnalyticAtGameStart()
    {
        SetEnabledAnalytic();
        if (willEnableAnalytic)
        {
            Analytics.ResumeInitialization();
        }
    }

    public void SetEnabledAnalytic()
    {
        Analytics.enabled = willEnableAnalytic;
        Analytics.deviceStatsEnabled = willEnableAnalytic;
        PerformanceReporting.enabled = willEnableAnalytic;
    }
}