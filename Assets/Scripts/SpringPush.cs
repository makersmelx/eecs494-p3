using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringPush : MonoBehaviour
{
    public float springStrength = 24f;
    // Start is called before the first frame update
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        Vector3 force = springStrength * Vector3.up;
        collision.gameObject.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
    }
}
