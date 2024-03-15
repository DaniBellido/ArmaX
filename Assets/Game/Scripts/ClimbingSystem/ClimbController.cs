using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbController : MonoBehaviour
{
    EnvironmentScanner environmentScanner;
    PlayerMovement pScript;
    ClimbPoint currentPoint;

    private void Awake()
    {
        pScript = GetComponent<PlayerMovement>();
        environmentScanner = GetComponent<EnvironmentScanner>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!pScript.IsHanging)
        {
            if (Input.GetKeyDown(KeyCode.Joystick1Button1) && !pScript.InAction)
            {
                if (environmentScanner.ClimbLedgeCheck(transform.forward, out RaycastHit ledgeHit))
                {
                    currentPoint = ledgeHit.transform.GetComponent<ClimbPoint>();

                    pScript.SetControl(false);
                    StartCoroutine(JumpToLedge("IdleToHang", ledgeHit.transform, 0.41f, 0.54f));
                }
            }
        }
        else 
        {
            if (Input.GetKeyDown(KeyCode.Joystick1Button3) && !pScript.InAction) 
            {
                StartCoroutine(JumpFromHang());
                return;
            }


            float h = Mathf.Round(Input.GetAxisRaw("Horizontal"));
            float v = Mathf.Round(Input.GetAxisRaw("Vertical"));
            var inputDirection = new Vector2(h, v);

            if (pScript.InAction && inputDirection == Vector2.zero) return;

            //MOUNT FROM HANGING STATE
            if (currentPoint.MountPoint && inputDirection.y == 1 && Input.GetKeyDown(KeyCode.Joystick1Button1)) 
            {
                StartCoroutine(MountFromHang());
                return;
            }

            //LEDGE TO LEDGE JUMP
            var neighbour = currentPoint.GetNeighbour(inputDirection);

            if (neighbour == null) return;

            if (neighbour.connectionType == ConnectionType.Jump && Input.GetKeyDown(KeyCode.Joystick1Button1))
            {
                currentPoint = neighbour.point;

                if (neighbour.direction.y == 1)
                    StartCoroutine(JumpToLedge("HangHopUp", currentPoint.transform, 0.34f, 0.65f));
                else if (neighbour.direction.y == -1)
                    StartCoroutine(JumpToLedge("HangHopDown", currentPoint.transform, 0.31f, 0.65f));
                else if (neighbour.direction.x == 1)
                    StartCoroutine(JumpToLedge("HangHopRight", currentPoint.transform, 0.20f, 0.50f));
                else if (neighbour.direction.x == -1)
                    StartCoroutine(JumpToLedge("HangHopLeft", currentPoint.transform, 0.20f, 0.50f));
            }
            else if (neighbour.connectionType == ConnectionType.Move && !pScript.InAction) 
            {
                currentPoint = neighbour.point;

                if (neighbour.direction.x == 1)
                    StartCoroutine(JumpToLedge("ShimmyRight", currentPoint.transform, 0.0f, 0.38f, handOffset: new Vector3(0.25f,0.05f,0.05f))); //handOffset can also be included in Jump connectionType to correct animations offset
                else if (neighbour.direction.x == -1)
                    StartCoroutine(JumpToLedge("ShimmyLeft", currentPoint.transform, 0.0f, 0.38f, AvatarTarget.LeftHand, handOffset: new Vector3(0.25f, 0.05f, 0.05f)));

            }

        }

       



    }

    IEnumerator JumpToLedge(string anim, Transform ledge, float matchStartTime, float matchTargetTime, 
        AvatarTarget hand = AvatarTarget.RightHand, Vector3? handOffset = null) 
    {
        var matchParams = new MatchTargetParams()
        {
            pos = GetHandPos(ledge, hand, handOffset),
            bodyPart = hand,
            startTime = matchStartTime,
            targetTime = matchTargetTime,
            posWeight = Vector3.one
        };

        var targetRotation = Quaternion.LookRotation(-ledge.forward);

        yield return pScript.DoAction(anim, matchParams, targetRotation, true);

        pScript.IsHanging = true;
    }

    Vector3 GetHandPos(Transform ledge, AvatarTarget hand, Vector3? handOffset) 
    {
        var offValue = (handOffset != null) ? handOffset.Value : new Vector3(0.25f, 0.05f, 0.05f);
        var hDir = (hand == AvatarTarget.RightHand) ? ledge.right : -ledge.right;
        //float values are offset correcting the animation's position
        return ledge.position + ledge.forward * offValue.z + Vector3.up * offValue.y - hDir * offValue.x;
    }

    IEnumerator JumpFromHang() 
    {
        pScript.IsHanging = false;
        yield return pScript.DoAction("JumpFromHang");
        pScript.SetControl(true);

    }

    IEnumerator MountFromHang() 
    {
        pScript.IsHanging = false;
        yield return pScript.DoAction("ClimbFromHang");

        pScript.EnableCharacterController(true);

        //yield return new WaitForSeconds(0.5f);

        pScript.SetControl(true);
    }
}
