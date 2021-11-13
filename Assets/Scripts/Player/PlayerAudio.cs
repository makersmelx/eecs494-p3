using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField] AudioClip[] JumpClips;

    private int jumpClipIndex = 0;

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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
