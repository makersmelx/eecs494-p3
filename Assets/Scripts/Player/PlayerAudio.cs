using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{

    [Header("Audio Sources")]
    [SerializeField] AudioSource jumpAudioSource;
    [SerializeField] AudioSource shieldAudioSource;
    [SerializeField] AudioSource playerHitSource;

    [Header("Audio Clips")]
    [SerializeField] AudioClip[] jumpClips;
    [SerializeField] AudioClip shieldActiveClip;
    [SerializeField] AudioClip shieldDeactivateClip;
    [SerializeField] AudioClip[] playerHitByBulletClips;
    [SerializeField] AudioClip shieldAbsorbBulletClip;

    private int jumpClipIndex = 0;
    private int playerHitByBulletIndex = 0;

    // -------------------------------------------------------------------------
    // Singleton
    // -------------------------------------------------------------------------
    private static PlayerAudio _instance;
    public static PlayerAudio Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this) Destroy(this.gameObject);
        else _instance = this;
    }

    // -------------------------------------------------------------------------
    // Public methods
    // -------------------------------------------------------------------------
    public void PlayJumpSound()
    {
        if (jumpClips.Length == 0) return;
        if (jumpAudioSource.isPlaying) return;

        jumpAudioSource.clip = jumpClips[jumpClipIndex];
        jumpAudioSource.Play();
        jumpClipIndex = (jumpClipIndex + 1) % jumpClips.Length;
    }

    public void PlayShieldActiveSound()
    {
        shieldAudioSource.clip = shieldActiveClip;
        shieldAudioSource.Play();
    }

    public void PlayShieldDeactiveSound()
    {
        shieldAudioSource.clip = shieldDeactivateClip;
        shieldAudioSource.Play();
    }

    public void PlayPlayerHitByBulletSound()
    {
        if (playerHitByBulletClips.Length == 0) return;

        playerHitSource.clip = playerHitByBulletClips[playerHitByBulletIndex];
        playerHitSource.Play();
        playerHitByBulletIndex = (playerHitByBulletIndex + 1) % playerHitByBulletClips.Length;
    }

    public void PlayPlayerShieldAbsorbBulletSound()
    {
        playerHitSource.clip = shieldAbsorbBulletClip;
        playerHitSource.Play();
    }
}
