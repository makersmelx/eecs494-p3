using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    // -------------------------------------------------------------------------
    // Component reference
    // -------------------------------------------------------------------------
    [Header("Component Reference")]
    [SerializeField] GameObject player;

    // -------------------------------------------------------------------------
    // Level Configuration
    // -------------------------------------------------------------------------
    [Header("Level Configuration")]
    [SerializeField] Vector3 playerStartPosition;
    [SerializeField] float fallResetHeight = 20f;

    // -------------------------------------------------------------------------
    // Internal State
    // -------------------------------------------------------------------------
    private Vector3 currentCheckpointPosition;

    // -------------------------------------------------------------------------
    // Singleton
    // -------------------------------------------------------------------------
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


    // -------------------------------------------------------------------------
    // Public methods
    // -------------------------------------------------------------------------
    public void UpdateCheckpoint(Vector3 checkpointPosition)
    {
        currentCheckpointPosition = checkpointPosition;
    }

    public void ResetAtCheckpoint()
    {
        // Send data to Unity Analytics
        CustomAnalyticsEvent.instance.CheckpointResetEvent(player.transform.position, Time.time);
        player.transform.position = currentCheckpointPosition;
    }

    public void OpenMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // -------------------------------------------------------------------------
    // Private methods
    // -------------------------------------------------------------------------
    void Start()
    {
        // Send data to Unity Analytics
        CustomAnalyticsEvent.instance.GameStartEvent(Time.time);
        player.transform.position = playerStartPosition;
        currentCheckpointPosition = playerStartPosition;
    }

    private void Update()
    {
        if (player.transform.position.y <= fallResetHeight)
        {
            ResetAtCheckpoint();
        }
    }
}
