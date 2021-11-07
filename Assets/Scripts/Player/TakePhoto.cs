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
    public GameObject flashImage;
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
            scopeImage.SetActive(true);
            playerCamera.fieldOfView = zoomScale;
        }
        else if (Input.GetMouseButtonDown(1) && inUse)
        {
            inUse = false;
            scopeImage.SetActive(false);
            playerCamera.fieldOfView = originalScale;
        }

        if (inUse)
        {
            // Scope_Image.SetActive(true);
            // check for flashing
            if (Input.GetMouseButtonDown(0) && flashCounter == 0f)
            {
                Debug.Log("Flash");
                flashImage.GetComponent<ImageFlash>().StartFlash(1f, 0.8f, Color.white);
                flashCounter += Time.deltaTime;
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
}
