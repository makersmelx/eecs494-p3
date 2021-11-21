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
    [SerializeField] bool isTurnable = true;
    [SerializeField] float cooldownTime = 2f;
    [SerializeField] float cdNoisePercent = 0.2f;
    [SerializeField] float rotationRate = 4f;
    
    [Header("Consecutive Shoot configuration")]
    [SerializeField] int numBullet = 1;
    [SerializeField] float timeInBetweenBullet = 0.25f;
    
    // -------------------------------------------------------------------------
    // Internal State
    // -------------------------------------------------------------------------
    private float actualCD;

    private SphereCollider sphereCollider;
    private AudioSource audioSource;
    private bool playerInRange = false;
    private Vector3 playerPosition;
    private float timeOfLastFire;
    IEnumerator currentShot;
    void Start()
    {
        // Set references
        sphereCollider = GetComponent<SphereCollider>();
        audioSource = GetComponent<AudioSource>();

        // Assign shoot audioclip
        audioSource.clip = turretShootSound;

        // Set time of last fire
        timeOfLastFire = Time.time + cooldownTime;

        //Set actual CD to cooldownTime
        actualCD = cooldownTime;
        //Prevent noise that is too large. 
        cdNoisePercent = cdNoisePercent > 1 ? 1 : cdNoisePercent;
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerInRange) return;

        // Point and shoot
        if (isTurnable)
        {
            PointAtPlayer();
        }
        Shoot();
    }

    private void PointAtPlayer()
    {
        Quaternion desiredRotation = Quaternion.LookRotation(playerPosition - gunTransform.position);
        gunTransform.rotation = Quaternion.Slerp(gunTransform.rotation, desiredRotation, Time.deltaTime * rotationRate);
    }

    private void Shoot()
    {
        if (Time.time - timeOfLastFire < actualCD) return;
        GenerateNextCD();
        
        // Not try to debug for overlap. If overlap, the designer should review if they enter the wrong time interval for bullets. 
        currentShot = ConsecutiveShoot();
        StartCoroutine(currentShot);

        timeOfLastFire = Time.time;
    }
    void GenerateNextCD()
    {
        //We add a small noise around 
        actualCD = cooldownTime + (Random.value - 0.5f) * 2 * cdNoisePercent;

    }

    IEnumerator ConsecutiveShoot()
    {
        // Create bullet instance, set rotation
        for (int i = 0; i < numBullet; i++)
        {
            Bullet bulletRef = Instantiate(bulletPrefab, gunHeadTransform.position, transform.rotation);
            bulletRef.SetBulletRotation(gunTransform.rotation);

            // Play shoot sound
            audioSource.Play();
            yield return new WaitForSeconds(timeInBetweenBullet);
        }
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
