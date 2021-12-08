using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallAfterTime : MonoBehaviour
{
    public float resumeTime;
    public float warningTime;

    public Material warningMat;
    public LayerMask whatShouldDetect;
    private Material normalMat;
    private MeshRenderer mr;
    private Collider coll;

    private bool isDisappearing = false;
    // Start is called before the first frame update
    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        coll = GetComponent<Collider>();
        normalMat = mr.material;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        StartCoroutine(Disappear());
    }

    private IEnumerator Disappear()
    {
        if (isDisappearing) 
            yield break;
        isDisappearing = true;
        mr.material = warningMat;
        yield return new WaitForSeconds(warningTime);
        mr.enabled = false;
        coll.isTrigger = true;
        isDisappearing = false;
        StartCoroutine(Appear());
    }

    private IEnumerator Appear()
    {
        yield return new WaitForSeconds(resumeTime);
        yield return new 
            WaitUntil(() => 
            Physics.OverlapBox(transform.position, new Vector3(1, 1, 1),Quaternion.identity,whatShouldDetect).Length == 0);
        mr.enabled = true;
        coll.isTrigger = false;
        mr.material = normalMat;
    }
}
