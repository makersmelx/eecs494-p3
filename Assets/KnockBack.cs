using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockBack : MonoBehaviour
{
    private Rigidbody Rigid;
    void Start()
    {
        Rigid = GetComponent<Rigidbody>();
    }

    public void KnockBackPlayer(Vector3 Dir, float Force)
    {
        Rigid.AddForce(Dir * Force, ForceMode.Impulse);
    }
}
