using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineOfSight : MonoBehaviour
{
   
    [SerializeField] LayerMask viewMask;
    [SerializeField] Transform[] bones;
    [SerializeField] int bonesToSpot = 2;
    [SerializeField] Transform target;
    [SerializeField] Light[] lights;
    Animator animator;

    [Header("Field Of View")]
    [Space(10)]
    [SerializeField] Vector3 gizmoPosition = Vector3.zero;
    [SerializeField] float viewDistance = 10f;
    [SerializeField] float viewAngle = 30f;
    [SerializeField] float RaycastYOffset = 0.5f;
    [SerializeField] float nearClippingPlane = 0.1f;
    [SerializeField] float farClippingPlane = 1f;
    [SerializeField] Color gizmoColor = Color.yellow;
    [SerializeField] bool showGizmo = true;

    bool targetSpotted = false;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        lights = GetComponentsInChildren<Light>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        gizmoPosition = transform.position;
        

        for (int i = 0; i < bones.Length; i++)
        {
            if (IsTargetInSight(bones[i]))
            {
                targetSpotted = true;
                break;
            }
            else
            {
                targetSpotted = false;
            }
        }

        if (targetSpotted)
        {
            Debug.Log("TARGET SPOTTED");
            animator.SetBool("isPatrolling", false);
            animator.SetBool("TargetSpotted", true);
            transform.LookAt(target);
        }
        else
        {
            Debug.Log("...");
            animator.SetBool("TargetSpotted", false);
        }

    }

    bool IsTargetInSight(Transform bone)
    {
        Vector3 direction = target.position - transform.position;
        float angle = Vector3.Angle(direction, transform.forward);
        if (angle < viewAngle / 2f)
        {
            Vector3 origin = transform.position;
            origin.y += RaycastYOffset;

            RaycastHit hit;
            if (Physics.Raycast(origin, direction.normalized, out hit, viewDistance, viewMask))
            {
                Debug.Log("Raycast hit " + hit.collider.name);
                Debug.DrawLine(origin, hit.point, Color.red);
                if (hit.collider.transform == target)
                {
                    return true;
                }
                
            }
            else
            {
                Debug.Log("Raycast did not hit anything");
                Debug.DrawLine(origin, bone.position + direction.normalized * viewDistance, Color.green);
            }
        }
        return false;

    }

    void OnDrawGizmos()
    {
        if (!showGizmo)
            return;

        Vector3 origin = transform.position + gizmoPosition;

        Gizmos.color = gizmoColor;
        //Gizmos.DrawWireSphere(origin, viewDistance);
        //Gizmos.DrawFrustum(origin, viewAngle, viewDistance, nearClippingPlane, farClippingPlane);
        for (float angle = 0; angle < viewAngle; angle += 0.1f)
        {
            float x = viewDistance * Mathf.Sin(angle);
            float z = viewDistance * Mathf.Cos(angle);
            Vector3 endPosition = origin + transform.forward * viewDistance + transform.right * x + transform.up * z;
            Gizmos.DrawLine(origin, endPosition);
        }
    }
}