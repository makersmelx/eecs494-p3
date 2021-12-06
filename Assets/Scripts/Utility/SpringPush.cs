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
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        rb.velocity = Vector3.Scale(Vector3.right + Vector3.forward, rb.velocity);
        rb.AddForce(force, ForceMode.Impulse);
    }
}
