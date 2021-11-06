using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakePhoto : MonoBehaviour
{
    [Header("Camera Zooming")]
    public GameObject Camera;
    public GameObject Scope_Image;
    public float ZoomScale = 30;
    private float OriginalScale;
    private bool InUse = false;

    [Header("Camera Flashing")]
    public GameObject FlashImage;
    public float FlashDuration = 1f;
    private float FlashCounter = 0f;

    void Start()
    {
        OriginalScale = Camera.GetComponent<Camera>().fieldOfView;
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetMouseButtonDown(1) && !InUse))
        {
            InUse = true;
            Scope_Image.SetActive(true);
            Camera.GetComponent<Camera>().fieldOfView = ZoomScale;
        }
        else if (Input.GetMouseButtonDown(1) && InUse)
        {
            InUse = false;
            Scope_Image.SetActive(false);
            Camera.GetComponent<Camera>().fieldOfView = OriginalScale;
        }

        if (InUse)
        {
            //Scope_Image.SetActive(true);
            // check for flashing
            if (Input.GetMouseButtonDown(0) && FlashCounter == 0f)
            {
                Debug.Log("Flash");
                FlashImage.GetComponent<ImageFlash>().StartFlash(1f, 0.8f, Color.white);
                FlashCounter += Time.deltaTime;
            }
            else if (FlashCounter != 0f)
            {
                FlashCounter += Time.deltaTime;
                if (FlashCounter >= FlashDuration)
                {
                    FlashCounter = 0f;
                }
               
            }
        }


    
    }
}
