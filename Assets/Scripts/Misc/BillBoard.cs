using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    // Start is called before the first frame update
    public float alternateTime = 2.5f;
    private bool isOn = false;
    public float hitBackMagnitude = 1f;
    public Color startColor;
    public Color stopColor;
    ParticleSystem spark;
    Material mat;

    public LayerMask whatIsPlayer;
    void Start()
    {
        spark = transform.Find("Spark").GetComponent<ParticleSystem>();
        mat = GetComponent<Renderer>().material;
        whatIsPlayer = ~whatIsPlayer;
        StartCoroutine(Blink());
    }

    IEnumerator Blink()
    {
        while (true)
        {
            isOn = true;
            spark.Play();
            //mat.color = new Color(0.5377358f, 0.533893f, 0.189676f, 1);
            mat.color = startColor;
            yield return new WaitForSeconds(alternateTime);
            isOn = false;
            spark.Stop();
            mat.color = stopColor;
            yield return new WaitForSeconds(alternateTime);
        }
    }


    private void OnCollisionStay(Collision collision)
    {
        Debug.Log("Is Touching...");
        if (!isOn) return;
        HasHealth healthBar = collision.gameObject.GetComponent<HasHealth>();
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        if (healthBar == null) return;
        healthBar.TakeDamage(1);
        rb.velocity = Vector3.zero;
        rb.AddForce(-hitBackMagnitude * collision.relativeVelocity, ForceMode.Impulse);
    }
}
