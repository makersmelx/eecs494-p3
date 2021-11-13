using System;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_CLOUD_SERVICES_ANALYTICS
using UnityEngine.Analytics;
#endif



public class CustomAnalyticsEvent : MonoBehaviour
{


    public string eventName = "Default Analytics Event Name";


    public void ReportEvent(Dictionary<string, object> infoDictionary)
    {
#if ENABLE_CLOUD_SERVICES_ANALYTICS
        Debug.Log("SENT INFORMATION TO ANALYTICS");
        Analytics.CustomEvent(eventName, infoDictionary);
#endif
    }


    /*
     A sample dictionary can be:  
     
     {
            { "potions", totalPotions },
            { "coins", totalCoins }
     }
     
     */

}
