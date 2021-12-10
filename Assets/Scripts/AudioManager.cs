using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource globalEffectsSource;

    // Sounds
    [SerializeField] AudioClip levelFailedClip;
    [SerializeField] AudioClip levelCompleteClip;


    private static AudioManager _instance;

    public static AudioManager Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public void PlayWinSound()
    {
        globalEffectsSource.clip = levelCompleteClip;
        globalEffectsSource.Play();
    }

    public void PlayFailSound()
    {
        globalEffectsSource.clip = levelFailedClip;
        globalEffectsSource.Play();
    }
}
