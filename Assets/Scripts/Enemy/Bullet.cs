using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Bullet : MonoBehaviour
{
    // -------------------------------------------------------------------------
    // Configurable params
    // -------------------------------------------------------------------------
    [Header("Bullet configuration")]
    [SerializeField] float velocity = 5f;
    [SerializeField] float maxLifetimeInSeconds = 10f;
    [SerializeField] AudioClip bulletFlyingSound;
    public float timeReductionOnHit = 4f;
    public float timeIncreaseOnHit = 4f;
    [Header("Accelaertaion configuration")]
    [SerializeField] bool isAccelerate = true;
    [SerializeField] float accelerationParameter = 2.5f;
    [SerializeField] float maxSpeed = 100f;
    // -------------------------------------------------------------------------
    // Internal State
    // -------------------------------------------------------------------------
    private float timeOfInstantiation;
    private AudioSource audioSource;

    // -------------------------------------------------------------------------
    // Public methods
    // -------------------------------------------------------------------------
    public void SetBulletRotation(Quaternion initRotation)
    {
        transform.rotation = initRotation;
    }


    // -------------------------------------------------------------------------
    // Private methods
    // -------------------------------------------------------------------------
    void Start()
    {
        timeOfInstantiation = Time.time;

        // Start bullet sound
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = bulletFlyingSound;
        audioSource.Play();
    }

    void Update()
    {
        // Destroy if alive for too long
        if (Time.time - timeOfInstantiation > maxLifetimeInSeconds)
        {
            Destroy(gameObject);
            return;
        }

        // Update position
        transform.position += transform.forward * (Time.deltaTime * velocity);

        if (isAccelerate && velocity < maxSpeed) Accelerate();
    }

    void Accelerate()
    {
        velocity += accelerationParameter * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Bullet collided with " + collision.gameObject.tag);
        Destroy(gameObject);
    }
}
