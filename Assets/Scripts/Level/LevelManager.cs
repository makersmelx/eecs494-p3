using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    // Player
    [SerializeField] GameObject player;
    [SerializeField] Vector3 playerStartPosition;
    [SerializeField] Vector3 currentCheckpointPosition;
    [SerializeField] float fallResetHeight = 20f;

    // UI
    [SerializeField] GameObject gameMenu;
    [SerializeField] GameObject toastManager;
    [SerializeField] GameObject winScreen;

    private bool levelComplete = false;


    private static LevelManager _instance;
    public static LevelManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else
        {
            _instance = this;
        }
    }

    void Start()
    {
        player.transform.position = playerStartPosition;
        winScreen.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (player.transform.position.y <= fallResetHeight)
        {
            ResetAtCheckpoint();
        }

        // Show/hide button depending on if playing
        gameMenu.gameObject.SetActive(!PlayerInputHandler.Instance.inGameMode);
        toastManager.gameObject.SetActive(PlayerInputHandler.Instance.inGameMode && !levelComplete);
    }

    public void UpdateCheckpoint(Vector3 checkpointPosition)
    {
        currentCheckpointPosition = checkpointPosition;
    }

    public void ResetAtCheckpoint()
    {
        player.transform.position = currentCheckpointPosition;
    }

    public void OpenMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ShowWinScreen()
    {
        winScreen.gameObject.SetActive(true);
        levelComplete = true;
        toastManager.gameObject.SetActive(false);
    }
}
