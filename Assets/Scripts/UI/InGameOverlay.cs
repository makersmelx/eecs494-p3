using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameOverlay : MonoBehaviour
{
    [SerializeField] GameObject toastManager;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject settingsMenu;

    // -------------------------------------------------------------------------
    // Public methods
    // -------------------------------------------------------------------------

    public void ResumeGame()
    {
        // Hide menus, activate toast
        PlayerInputHandler.Instance.EnterGameMode();
        pauseMenu.gameObject.SetActive(false);
        settingsMenu.gameObject.SetActive(false);
        toastManager.gameObject.SetActive(true);
    }

    public void PauseGame()
    {
        // Show pause menu
        PlayerInputHandler.Instance.ExitGameMode();
        pauseMenu.gameObject.SetActive(true);
        settingsMenu.gameObject.SetActive(false);
        toastManager.gameObject.SetActive(false);
    }

    public void OpenMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ToggleSettings()
    {
        bool isActive = settingsMenu.activeSelf;
        settingsMenu.SetActive(!isActive);
    }

    // -------------------------------------------------------------------------
    // Private methods
    // -------------------------------------------------------------------------

    // Start is called before the first frame update
    void Start()
    {
        if (toastManager == null) Debug.LogError("Assign the toast manager in the InGameOverlay component");
        if (pauseMenu == null) Debug.LogError("Assign the pause menu in the InGameOverlay component");
        if (settingsMenu == null) Debug.LogError("Assign the settings menu in the InGameOverlay component");

        ResumeGame();
    }

    private void Update()
    {
        if (PlayerInputHandler.Instance.GetEscapeButtonDown())
        {
            PauseGame();
        }
    }
}