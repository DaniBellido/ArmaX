using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsUtil 
{
    public static bool ThreeRayCasts(Vector3 origin, Vector3 dir, float spacing, Transform transform,
        out List<RaycastHit> hits, float distance, LayerMask layer, bool debugDraw = false)
    {
        bool centerHitFound = Physics.Raycast(origin, Vector3.down, out RaycastHit centerHit, distance, layer);
        bool leftHitFound = Physics.Raycast(origin - transform.right * spacing, Vector3.down, out RaycastHit leftHit, distance, layer);
        bool rightHitFound = Physics.Raycast(origin + transform.right * spacing, Vector3.down, out RaycastHit rightHit, distance, layer);

        hits = new List<RaycastHit>() { centerHit, leftHit, rightHit };

        bool hitfound = centerHitFound || leftHitFound || rightHitFound;

        if (hitfound && debugDraw) 
        {
            Debug.DrawLine(origin, centerHit.point, Color.red);
            Debug.DrawLine(origin - transform.right * spacing, leftHit.point, Color.red);
            Debug.DrawLine(origin + transform.right * spacing, rightHit.point, Color.red);
        }

        return hitfound;
       
    }

 
}
