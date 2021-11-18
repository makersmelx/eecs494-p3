using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Play()
    {

        // Send data to Unity Analytics
        CustomAnalyticsEvent.instance.GameStartEvent(Time.time);
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Quit()
    {
        Debug.Log("Quit");

        // Send data to Unity Analytics
        CustomAnalyticsEvent.instance.GameQuitEvent(Time.time);

        Application.Quit();
    }
}
