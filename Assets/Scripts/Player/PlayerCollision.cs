using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public float FloorCheckRadius;
    public float bottomOffset;
    public float WallCheckRadius;
    public float frontOffset;
    public float RoofCheckRadius;
    public float upOffset;

    public float LedgeGrabForwardPos;
    public float LedgeGrabUpwardPos;
    public float LedgeGrabDistance;

    public LayerMask FloorLayers;
    public LayerMask WallLayers;
    public LayerMask RoofLayers;
    public LayerMask LedgeGrabLayers;


    public bool CheckFloors(Vector3 dir)
    {
        Vector3 pos = transform.position + dir * bottomOffset;
        Collider[] ColHit = Physics.OverlapSphere(pos, FloorCheckRadius, FloorLayers);
        if (ColHit.Length > 0)
        {
            return true;
        }
        return false;

    }

    public bool CheckWalls(Vector3 dir)
    {
        Vector3 pos = transform.position + dir * frontOffset;
        Collider[] ColHit = Physics.OverlapSphere(pos, WallCheckRadius, WallLayers);
        if (ColHit.Length > 0)
        {
            return true;
        }
        return false;

    }

    public Vector3 CheckLedges()
    {
        Vector3 RayPos = transform.position + (transform.forward * LedgeGrabForwardPos) + (transform.up * LedgeGrabUpwardPos);

        RaycastHit Hit;
        if (Physics.Raycast(RayPos, -transform.up, out Hit, LedgeGrabDistance, LedgeGrabLayers))
        {
            return Hit.point;
        }

        return Vector3.zero;
    }

    public bool CheckRoof(Vector3 Dir)
    {
        Vector3 pos = transform.position + Dir * upOffset;

        Collider[] ColHit = Physics.OverlapSphere(pos, RoofCheckRadius, RoofLayers);
        if (ColHit.Length > 0)
        {
            return true;
        }
        return false;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 pos = transform.position + (-transform.up * bottomOffset);
        Gizmos.DrawSphere(pos, FloorCheckRadius);

        Gizmos.color = Color.red;
        pos = transform.position + (transform.forward * frontOffset);
        Gizmos.DrawSphere(pos, WallCheckRadius);

        Gizmos.color = Color.cyan;
        pos = transform.position + transform.forward * LedgeGrabForwardPos + transform.up * LedgeGrabUpwardPos;
        Gizmos.DrawLine(pos, pos + transform.up);
    }
}
