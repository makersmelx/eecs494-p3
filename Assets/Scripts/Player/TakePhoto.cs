using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakePhoto : MonoBehaviour
{
    [Header("Camera Zooming")]
    public Camera playerCamera;
    public GameObject scopeImage;
    public float zoomScale = 30;
    private float originalScale;
    private bool inUse = false;

    [Header("Camera Flashing")]
    public ImageFlash imageFlash;
    public float flashDuration = 1f;
    private float flashCounter = 0f;

    [Header("Camera Audio")]
    [SerializeField] AudioClip flashAudio;

    void Start()
    {
        originalScale = playerCamera.fieldOfView;
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetMouseButtonDown(1) && !inUse))
        {
            inUse = true;
            //scopeImage.SetActive(true);
            playerCamera.fieldOfView = zoomScale;
        }
        else if (Input.GetMouseButtonDown(1) && inUse)
        {
            inUse = false;
            //scopeImage.SetActive(false);
            playerCamera.fieldOfView = originalScale;
        }

        if (inUse)
        {
            // check for flashing
            if (Input.GetMouseButtonDown(0) && flashCounter == 0f)
            {
                Debug.Log("Flash");
                imageFlash.StartFlash(flashDuration, 0.8f, Color.white);
                flashCounter += Time.deltaTime;
                AudioManager.Instance.Play(flashAudio);
                Capture();
            }
            else if (flashCounter != 0f)
            {
                flashCounter += Time.deltaTime;
                if (flashCounter >= flashDuration)
                {
                    flashCounter = 0f;
                }
            }
        }
    }

    // todo (#33): this is only a temp solution for triggering winning, an issue is created to modify this, check #33 for details
    void Capture()
    {
        RaycastHit Hit;

        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out Hit))
        {
            
            GameObject castObj = Hit.transform.gameObject;
            Debug.Log(castObj.tag);
            if (castObj.CompareTag("WinTrigger"))
            {
                Debug.Log("Win Trigger Detected");
                // TO DO: IMPLEMENT WIN TRIGGER HERE
                // todo (#33): this is only a temp solution for triggering winning, an issue is created to modify this, check #33 for details
                LevelManager.Instance.ShowWinScreen();
            }
        }
    }
}
