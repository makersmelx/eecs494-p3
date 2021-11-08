using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] Vector3 playerStartPosition;
    [SerializeField] Vector3 currentCheckpointPosition;
    [SerializeField] float fallResetHeight = 20f;

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
    }

    private void Update()
    {
        if (player.transform.position.y <= fallResetHeight)
        {
            ResetAtCheckpoint();
        }
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
}
