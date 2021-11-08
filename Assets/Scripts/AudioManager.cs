using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public AudioSource EffectsSource;
	public static AudioManager Instance = null;

	private void Awake()
	{
		if (Instance == null) Instance = this;
		else if (Instance != this) Destroy(gameObject);
	}

	public void Play(AudioClip clip)
	{
		EffectsSource.clip = clip;
		EffectsSource.Play();
	}
}
