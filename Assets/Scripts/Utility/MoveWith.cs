using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWith : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform target;
    public bool followX;
    public bool followY;
    public bool followZ;
    public float xOffset = 0;
    public float yOffset = 0;
    public float zOffset = 0;


    // Update is called once per frame

    void Update()
    {
        transform.position = new Vector3(followX ? target.position.x + xOffset : transform.position.x,
            followY ? target.position.y + yOffset : transform.position.y,
            followZ ? target.position.z +zOffset : transform.position.z);
    }
}
