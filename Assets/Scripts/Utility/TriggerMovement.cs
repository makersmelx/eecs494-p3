using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerMovement : MonoBehaviour
{

    private bool isTriggered = false;
    private Vector3 initPos;
    public Transform target;
    public Vector3 offset;
    public float duration;

    private void Start()
    {
        initPos = target.position;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!isTriggered)
        {
            isTriggered = true;
            StartCoroutine(Move());
        }
    }
    IEnumerator Move()
    {
        float initTime = Time.time;
        float progress = (Time.time - initTime) / duration;
        while (progress < 1.0)
        {
            progress = (Time.time - initTime) / duration;
            Vector3 newOffset = Vector3.Lerp(Vector3.zero, offset, progress);
            target.position = initPos + newOffset;
            yield return null;
        }
        target.position = initPos + offset;
    }
}
