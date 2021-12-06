using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDeath : MonoBehaviour
{
    // Start is called before the first frame update
    //This is a test code that is temporary to automatically respawn the main player.
    Vector3 initPos;
    private Quaternion initRotation;
    private Coroutine currentCoroutine;
    private bool isDead = false;

    public GameObject timeUpUI;
    [SerializeField] string timeUpUIName = "TimeUp";

    void Start()
    {
        Transform playerTransform = PlayerMoveControl.Instance.transform;
        initPos = playerTransform.position;
        initRotation = playerTransform.rotation;
        TimeManager.Instance.TimeUpEffect += Die;
        timeUpUI = Instantiate(Resources.Load<GameObject>("prefabs/UI/" + timeUpUIName),
            GameObject.Find("Canvas").transform);
    }

    public void Die()
    {
        if (isDead || timeUpUI.activeInHierarchy)
        {
            return;
        }

        isDead = true;
        LevelManager.Instance.UpdateCheckpoint(initPos, Quaternion.identity);
        LevelManager.Instance.currentTrial += 1;
        CustomAnalyticsEvent.instance.LevelFailedEvent(transform.position, Time.time);
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        
        timeUpUI.SetActive(true);
        currentCoroutine = StartCoroutine(DeathCoroutine());
        //TimeManager.Instance.ResetTimer();
    }

    IEnumerator DeathCoroutine()
    {
        yield return timeUpUI.GetComponent<TimeUpUI>().DeathSceneTransaction();
        PlayerMoveControl.Instance.transform.position = initPos;
        PlayerMoveControl.Instance.transform.rotation = initRotation;
        isDead = false;
    }
}