using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] Vector3 playerStartPosition;
    [SerializeField] Vector3 currentCheckpointPosition;
    [SerializeField] float fallResetHeight = 20f;
    public bool isDead = false;
    public GameObject resetPanelPrefab;

    private GameObject resetPanel;
    private Coroutine resetCoroutine;
    private bool canReset = true;

    private static LevelManager _instance;

    public static LevelManager Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    void Start()
    {
        player.transform.position = playerStartPosition;
        currentCheckpointPosition = playerStartPosition;
        resetPanel = Instantiate(resetPanelPrefab, GameObject.Find("Canvas").transform);
    }

    private void Update()
    {
        if (player.transform.position.y <= fallResetHeight && canReset)
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
        if (resetCoroutine != null)
        {
            StopCoroutine(resetCoroutine);
        }

        resetCoroutine = StartCoroutine(ResetInCoroutine());
    }

    IEnumerator ResetInCoroutine()
    {
        // Send data to Unity Analytics
        CustomAnalyticsEvent.instance.CheckpointResetEvent(player.transform.position, Time.time);

        // disable input, also make sure this coroutine works only once
        canReset = false;
        isDead = true;

        resetPanel.SetActive(true);
        PlayerInputHandler.Instance.EnterGameMode();

        yield return CameraFade(true, 0.7f);

        player.transform.position = currentCheckpointPosition;
        PlayerInputHandler.Instance.EnterGameMode();

        yield return new WaitForSeconds(1.5f);

        yield return CameraFade(false, 0.7f);

        resetPanel.SetActive(false);

        isDead = false;
        canReset = true;
    }

    IEnumerator CameraFade(bool fadeIn, float duration)
    {
        float start = Time.time;
        float progress = (Time.time - start) / duration;
        while (progress < 1f)
        {
            progress = (Time.time - start) / duration;

            float alpha = (float) (fadeIn ? Math.Sqrt(progress) : Math.Sqrt(1 - progress));

            resetPanel.GetComponent<Image>().color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }
    }

    public void OpenMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}