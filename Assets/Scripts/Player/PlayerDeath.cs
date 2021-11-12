using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDeath : MonoBehaviour
{
    // Start is called before the first frame update
    //This is a test code that is temporary to automatically respawn the main player.
    Vector3 initPos;
    private Quaternion initRotation;

    GameObject panelHealth;
    Text healthText;
    void Start()
    {
        initPos = transform.position;
        initRotation = transform.rotation;
        TimeManager.instance.TimeUpEffect += Die;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -10f)
        {
            FallReset();
        }
    }
    void FallReset()
    {
        transform.position = initPos;
        transform.rotation = initRotation;
        TimeManager.instance.ReduceTime(1f);
    }
    public void Die()
    {
        transform.position = initPos;
        transform.rotation = initRotation;
        TimeManager.instance.ResetTimer();
    }

}
