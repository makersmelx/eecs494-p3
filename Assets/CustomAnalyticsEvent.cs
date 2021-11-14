using System;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_CLOUD_SERVICES_ANALYTICS
using UnityEngine.Analytics;
#endif



public class CustomAnalyticsEvent : MonoBehaviour
{


  
    public void ReportEvent(string eventName, string key, object value)
    {

        #if ENABLE_CLOUD_SERVICES_ANALYTICS
        Dictionary<string, object> eventDict = new Dictionary<string, object>{
            {key , value}
        };
        Analytics.CustomEvent(eventName, eventDict);
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
