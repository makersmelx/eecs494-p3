using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOneDirection : MonoBehaviour
{
    public Vector3 initPos;
    public Vector3 endPos;

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
        if((transform.position - endPos).magnitude<0.5f || (transform.position - endPos).magnitude>dist * 1.2f)
        {
            transform.position = initPos;
        }
    }
}
