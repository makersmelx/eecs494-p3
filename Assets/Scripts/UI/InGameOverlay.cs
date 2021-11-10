using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameOverlay : MonoBehaviour
{
    [SerializeField] GameObject toastManager;
    [SerializeField] GameObject pauseMenu;

    // -------------------------------------------------------------------------
    // Public methods
    // -------------------------------------------------------------------------

    public void ResumeGame()
    {
        PlayerInputHandler.Instance.EnterGameMode();
    }

    public void PauseGame()
    {
        PlayerInputHandler.Instance.ExitGameMode();
    }

    public void OpenMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // -------------------------------------------------------------------------
    // Private methods
    // -------------------------------------------------------------------------

    // Start is called before the first frame update
    void Start()
    {
        if (toastManager == null) Debug.LogError("Assign the toast manager in the InGameOverlay component");
        if (pauseMenu == null) Debug.LogError("Assign the pause menu in the InGameOverlay component");
    }

    // Update is called once per frame
    void Update()
    {
        if (!ReferencesSet()) return;

        // Player is playing game
        if (PlayerInputHandler.Instance.inGameMode)
        {
            pauseMenu.gameObject.SetActive(false);      // Hide pause menu
            toastManager.gameObject.SetActive(true);    // Show toast manager
        }

        // Pause menu is open
        if (!PlayerInputHandler.Instance.inGameMode)
        {
            pauseMenu.gameObject.SetActive(true);       // Show pause menu 
            toastManager.gameObject.SetActive(false);   // Hide toast manager
        }
    }

    private bool ReferencesSet()
    {
        bool referencesSet = true;
        if (toastManager == null) referencesSet = false;
        if (pauseMenu == null) referencesSet = false;
        return referencesSet;
    }
}
