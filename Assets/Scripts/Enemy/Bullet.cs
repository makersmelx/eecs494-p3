using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Bullet : MonoBehaviour
{
    // -------------------------------------------------------------------------
    // Configurable params
    // -------------------------------------------------------------------------
    [Header("Bullet configuration")] [SerializeField]
    float velocity = 5f;

    [SerializeField] float maxLifetimeInSeconds = 10f;
    [SerializeField] AudioClip bulletFlyingSound;
    public float timeReductionOnHit = 4f;
    public float timeIncreaseOnHit = 4f;

    [Header("Accelaertaion configuration")] [SerializeField]
    bool isAccelerate = true;

    [SerializeField] float accelerationParameter = 2.5f;

    [SerializeField] float maxSpeed = 100f;

    // -------------------------------------------------------------------------
    // Internal State
    // -------------------------------------------------------------------------
    private float timeOfInstantiation;
    private AudioSource audioSource;
    private float timer;

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
        timer = Time.time;
    }

    void Update()
    {
        timer += Time.deltaTime * LevelManager.Instance.timeScale;
        // Destroy if alive for too long
        if (timer - timeOfInstantiation > maxLifetimeInSeconds)
        {
            Destroy(gameObject);
            return;
        }

        // Update position
        transform.position += transform.forward * (Time.deltaTime * LevelManager.Instance.timeScale * velocity);

        if (isAccelerate && velocity < maxSpeed) Accelerate();
    }

    void Accelerate()
    {
        velocity += accelerationParameter * Time.deltaTime * LevelManager.Instance.timeScale;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Bullet collided with " + collision.gameObject.tag);
        Destroy(gameObject);
    }
}