using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantRotation : MonoBehaviour
{
    [SerializeField]
    float degreesPerSecond = 1f;

    public void Start()
    {
        StartCoroutine(Rotation());
    }

    IEnumerator Rotation()
    {
        while(true)
        {
            transform.Rotate(0, degreesPerSecond * Time.deltaTime, 0);
            yield return null;
        }
    }
}
