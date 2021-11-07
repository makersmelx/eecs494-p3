using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallReset : MonoBehaviour
{
    // Start is called before the first frame update
    // This is a test code that is temporary to automatically respawn the main player.
    Vector3 initPos;

    void Start()
    {
        initPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -25f)
        {
            transform.position = initPos;
        }
    }
}
