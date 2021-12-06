using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class TeleportPortal : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform[] dest;
    public int index;
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        collision.gameObject.transform.position = dest[index].position;
    }
}
