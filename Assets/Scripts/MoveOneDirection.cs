using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOneDirection : MonoBehaviour
{
    public Vector3 initPos;
    public Vector3 endPos;
    public bool isBackAndForth;
    [Tooltip("How much percent of distance it travels per second from initPos to endPos")]
    public float travelRate= 0.33f;

    Vector3 velocity;
    float dist;
    // Start is called before the first frame update
    void Start()
    {
        dist = (endPos - initPos).magnitude;
        velocity = (endPos - initPos) * travelRate;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += velocity * Time.deltaTime;
        if((transform.position - endPos).magnitude< 0.1f || (transform.position - initPos).magnitude > dist * 1.1f)
        {
            if (isBackAndForth)
            {
                transform.position = endPos;
                endPos = initPos;
                initPos = transform.position;
                dist = (endPos - initPos).magnitude;
                velocity = (endPos - initPos) * travelRate;
            }
            else
            {
                transform.position = initPos;
            }
        }
    }
}
