using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourSystem : MonoBehaviour
{
    EnvironmentScanner environmentScanner;
    Animator pAnimator;
    PlayerMovement pScript;

    [SerializeField] List<ParkourAction> parkourActions;
    [SerializeField] ParkourAction jumpDownAction;
    [SerializeField] float autoJumpHeightLimit = 1;

   
    private void Awake()
    {
        environmentScanner = GetComponent<EnvironmentScanner>();
        pAnimator = GetComponent<Animator>();
        pScript = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        var hitData = environmentScanner.ObstacleCheck();

        if (Input.GetKeyDown(KeyCode.Joystick1Button1) && !pScript.InAction && !pScript.IsHanging)
        {


            if (hitData.forwardHitFound)
            {
                foreach (var action in parkourActions)
                {
                    if (action.CheckIfPossible(hitData, transform))
                    {
                        StartCoroutine(DoAction(action));
                        Debug.Log("Obstacle Found " + hitData.forwardHit.transform.name);
                        break;
                    }
                }
            }
        }

        if (pScript.IsOnLedge && !pScript.InAction && !hitData.forwardHitFound)
        {
            bool shouldJump = true;
            if (pScript.LedgeData.height > autoJumpHeightLimit && !Input.GetKey(KeyCode.Joystick1Button1))
                shouldJump = false;

            if (shouldJump && pScript.LedgeData.angle <= 50)
            {
                pScript.IsOnLedge = false;
                StartCoroutine(DoAction(jumpDownAction));
            }
        }
    }

    IEnumerator DoAction(ParkourAction action)
    {
        pScript.SetControl(false);

        MatchTargetParams matchParams = null;
        if (action.EnableTargetMatching)
        {
            matchParams = new MatchTargetParams()
            {
                pos = action.MatchPosition,
                bodyPart = action.MatchBodyPart,
                posWeight = action.MatchPositionWeight,
                startTime = action.MatchStartTime,
                targetTime = action.MatchTargetTime
            };
        }

        yield return pScript.DoAction(action.AnimName, matchParams, action.TargetRotation, 
            action.RotateToObstacle, action.PostActionDelay, action.Mirror);

        pScript.SetControl(true);
    }

 
    

    

    void MatchTarget(ParkourAction action) 
    {
        if (pAnimator.isMatchingTarget || pAnimator.IsInTransition(0)) return;

        pAnimator.MatchTarget(action.MatchPosition, transform.rotation, action.MatchBodyPart, 
            new MatchTargetWeightMask(action.MatchPositionWeight, 0), action.MatchStartTime, action.MatchTargetTime);
    }
}
