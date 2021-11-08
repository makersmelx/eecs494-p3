using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringShake : MonoBehaviour
{
    // Start is called before the first frame update

    private bool isShake = false;
    private Vector3 initPos;


    public float detectThreshold =5f;
    public float shakeTime = 3f;
    public float initVelocity = 0.6f;
    public float springConstant = 0.5f;
    public float dampeningConsstant = 0.98f;
    void Start()
    {
        initPos = transform.position;
    }
   
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision Speed is " + collision.relativeVelocity);
        if (Mathf.Abs(collision.relativeVelocity.y)  < detectThreshold) return;
        if (isShake) StopAllCoroutines();
        StartCoroutine(Shake(Mathf.Sign(collision.relativeVelocity.y)));
    }

    IEnumerator Shake(float direction)
    {
        isShake = true;
        Vector3 velocity = initVelocity * direction * Vector3.up;
        transform.position += velocity;
        Vector3 displacement, acceleration ;
        float initTime = Time.time;
        float progress = (Time.time - initTime) / shakeTime;
        while (progress < 1.0f)
        {
            progress = (Time.time - initTime) / shakeTime;
            displacement = (initPos.y - transform.position.y) * Vector3.up;
            acceleration = displacement * springConstant;
            velocity += acceleration;
            velocity *= dampeningConsstant;
            transform.position += velocity;
            yield return null;
            yield return null;
        }
        transform.position= new Vector3(transform.position.x,initPos.y,transform.position.z);
        isShake = false;
    }
}
