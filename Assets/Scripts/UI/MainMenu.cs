using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Play()
    {
        Debug.Log("MAIN MENU BUTTON PRESSED");
        Dictionary<string, object> eventDict = new Dictionary<string, object>{

            
            {"start time" , Time.time}

        };

        GetComponent<CustomAnalyticsEvent>().ReportEvent(eventDict);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Quit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
