using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(AudioSource))]
public class Turret : MonoBehaviour
{
    // -------------------------------------------------------------------------
    // Component / Game Object Reference
    // -------------------------------------------------------------------------
    [Header("Component references")] [SerializeField]
    Transform gunTransform;

    [SerializeField] Transform gunHeadTransform;
    [SerializeField] Bullet bulletPrefab;
    [SerializeField] AudioClip turretShootSound;

    // -------------------------------------------------------------------------
    // Configurable params
    // -------------------------------------------------------------------------
    [Header("Turret configuration")] [SerializeField]
    bool isTurnable = true;

    [SerializeField] float cooldownTime = 2f;
    [SerializeField] float cdNoisePercent = 0.2f;
    [SerializeField] float rotationRate = 4f;

    [Header("Consecutive Shoot configuration")] [SerializeField]
    int numBullet = 1;

    [SerializeField] float timeInBetweenBullet = 0.25f;

    [Header("In which range around the player will the turret aim at randomly")]
    public float randomAimRange = 4f;

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

    private float timeOfLastBullet;
    private int currentBullet;
    private bool isShooting;

    private float timer;

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
        timer = Time.time;
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
        timer += Time.deltaTime * LevelManager.Instance.timeScale;
    }

    private void PointAtPlayer()
    {
        Quaternion desiredRotation = Quaternion.LookRotation(playerPosition - gunTransform.position);
        gunTransform.rotation = Quaternion.Slerp(gunTransform.rotation, desiredRotation,
            Time.deltaTime * LevelManager.Instance.timeScale * rotationRate);
    }

    private void Shoot()
    {
        // Not try to debug for overlap. If overlap, the designer should review if they enter the wrong time interval for bullets. 
        if (timer - timeOfLastFire < actualCD && !isShooting) return;

        if (!isShooting)
        {
            isShooting = true;
            GenerateNextCD();
        }
        else
        {
            if (timer - timeOfLastBullet < timeInBetweenBullet)
            {
                return;
            }

            Bullet bulletRef = Instantiate(
                bulletPrefab,
                gunHeadTransform.position + gunHeadTransform.forward * 1f,
                transform.rotation
            );

            bulletRef.SetBulletRotation(gunTransform.rotation);
            // Play shoot sound
            audioSource.Play();
            timeOfLastBullet = timer;
            currentBullet += 1;
            if (currentBullet == numBullet)
            {
                timeOfLastFire = timer;
                isShooting = false;
                currentBullet = 0;
            }
        }
    }

    void GenerateNextCD()
    {
        //We add a small noise around 
        actualCD = cooldownTime + (Random.value - 0.5f) * 2 * cdNoisePercent;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!playerInRange)
            {
                timeOfLastFire -= actualCD;
            }

            playerInRange = true;
            playerPosition = other.transform.position;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) playerInRange = false;
    }
}