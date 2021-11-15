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
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Bullet collided with " + collision.gameObject.tag);
        Destroy(gameObject, .1f);
    }
}
