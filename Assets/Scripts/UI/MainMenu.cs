using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject settingsMenu;

    public void Play()
    {
        CustomAnalyticsEvent.instance.ConfigureAnalyticAtGameStart();
        // Send data to Unity Analytics
        CustomAnalyticsEvent.instance.GameStartEvent(Time.time);

        if (GameConstants.playCutScene >= 1)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    public void ToggleSettings()
    {
        bool isActive = settingsMenu.activeSelf;
        settingsMenu.SetActive(!isActive);
    }

    public void Quit()
    {
        Debug.Log("Quit");

        // Send data to Unity Analytics
        CustomAnalyticsEvent.instance.GameQuitEvent(Time.time);

        Application.Quit();
    }
}