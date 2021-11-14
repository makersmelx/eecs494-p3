using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    private int buttonClickTime = 0;
    public void Play()
    {

        buttonClickTime += 1;
        GetComponent<CustomAnalyticsEvent>().ReportEvent("Game Start", "count", buttonClickTime);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Quit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
