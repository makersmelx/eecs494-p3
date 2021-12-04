using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstructionDetection : MonoBehaviour
{
    public string targetTagName = "";
    public bool isObstructed;
    public GameObject target;
    public Collider currentCollider;

    private void OnTriggerStay(Collider other)
    {
        CustomTag customTag = other.GetComponent<CustomTag>();
        bool canTagMatch = customTag != null && customTag.isEnabled;
        bool isTarget = (targetTagName != "" && canTagMatch)
                        || targetTagName == "";

        if (!isObstructed)
        {
            if (other != null && !other.isTrigger && isTarget)
            {
                isObstructed = true;
                target = other.gameObject;
                currentCollider = other;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == currentCollider)
        {
            isObstructed = false;
        }
    }

    private void Update()
    {
        if (target == null || !currentCollider.enabled)
        {
            isObstructed = false;
        }

        if (target != null && !target.activeInHierarchy)
        {
            isObstructed = false;
        }
    }
}