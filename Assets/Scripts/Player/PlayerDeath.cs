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

    GameObject panelDie;
    [SerializeField] string panelDiePrefabName = "PanelDie";

    // Record player death times
    void Start()
    {
        initPos = transform.position;
        initRotation = transform.rotation;
        TimeManager.Instance.TimeUpEffect += Die;
        panelDie = Resources.Load<GameObject>("prefabs/UI/" + panelDiePrefabName);
        panelDie = Instantiate(panelDie, GameObject.Find("Canvas").transform);
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
        TimeManager.Instance.ReduceTime(1f);
    }
    public void Die()
    {
        transform.position = initPos;
        transform.rotation = initRotation;
        panelDie.SetActive(true);
        //TimeManager.Instance.ResetTimer();
    }
}
