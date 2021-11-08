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
                imageFlash.StartFlash(1f, 0.8f, Color.white);
                flashCounter += Time.deltaTime;
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

    void Capture()
    {
        RaycastHit Hit;

        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out Hit))
        {
            GameObject castObj = Hit.transform.gameObject;
            if (castObj.CompareTag("WinTrigger"))
            {
                Debug.Log("Win Trigger Detected");
                // TO DO: IMPLEMENT WIN TRIGGER HERE

            }
        }
    }
}
