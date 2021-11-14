using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(AudioSource))]
public class Turret : MonoBehaviour
{
    // -------------------------------------------------------------------------
    // Component / Game Object Reference
    // -------------------------------------------------------------------------
    [Header("Component references")]
    [SerializeField] Transform gunTransform;
    [SerializeField] Transform gunHeadTransform;
    [SerializeField] Bullet bulletPrefab;
    [SerializeField] AudioClip turretShootSound;

    // -------------------------------------------------------------------------
    // Configurable params
    // -------------------------------------------------------------------------
    [Header("Turret configuration")]
    [SerializeField] float cooldownTime = 2f;
    [SerializeField] float rotationRate = 4f;

    // -------------------------------------------------------------------------
    // Internal State
    // -------------------------------------------------------------------------
    private SphereCollider sphereCollider;
    private AudioSource audioSource;
    private bool playerInRange = false;
    private Vector3 playerPosition;
    private float timeOfLastFire;

    void Start()
    {
        // Set references
        sphereCollider = GetComponent<SphereCollider>();
        audioSource = GetComponent<AudioSource>();

        // Assign shoot audioclip
        audioSource.clip = turretShootSound;

        // Set time of last fire
        timeOfLastFire = Time.time + cooldownTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerInRange) return;

        // Point and shoot
        PointAtPlayer();
        Shoot();
    }

    private void PointAtPlayer()
    {
        Quaternion desiredRotation = Quaternion.LookRotation(playerPosition - gunTransform.position);
        gunTransform.rotation = Quaternion.Slerp(gunTransform.rotation, desiredRotation, Time.deltaTime * rotationRate);
    }

    private void Shoot()
    {
        if (Time.time - timeOfLastFire < cooldownTime) return;

        // Create bullet instance, set rotation
        Bullet bulletRef = Instantiate(bulletPrefab, gunHeadTransform.position, transform.rotation);
        bulletRef.SetBulletRotation(gunTransform.rotation);
        timeOfLastFire = Time.time;

        // Play shoot sound
        audioSource.Play();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            playerPosition = other.transform.position;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player")) playerInRange = false;
    }
}
