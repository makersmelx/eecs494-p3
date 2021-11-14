using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void Play()
    {

        // send info to Unity Analytics
        GetComponent<CustomAnalyticsEvent>().ReportEvent("Game Start", "time", Time.time);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Quit()
    {
        
        Debug.Log("Quit");
        
        // send info to Unity Analytics
        GetComponent<CustomAnalyticsEvent>().ReportEvent("Game Quit", "time", Time.time);

        Application.Quit();
    }
}
