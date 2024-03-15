using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Parkour System/Custom Actions/New vault action")]
public class VaultAction : ParkourAction
{
    public override bool CheckIfPossible(ObstacleHitData hitData, Transform player)
    {
        if (!base.CheckIfPossible(hitData, player))
            return false;

        var hitPoint= hitData.forwardHit.transform.InverseTransformPoint(hitData.forwardHit.point);

        //IMPORTANT TO KEEP FENCE'S Z COORDINATE AS FORWARD IN LOCAL SPACE AND PIVOT POINT IN THE CENTER OF THE OBJECT!

        if (hitPoint.z < 0 && hitPoint.x < 0 || hitPoint.z > 0 && hitPoint.x > 0)
        {
            //IF player approaches to the obstacle from the back and from the left side
            //OR player approaches to the obstacle from the front and from the right side
            //mirror animation

            Mirror = true;
            matchBodyPart = AvatarTarget.RightHand;

        }
        else 
        {
            //Dont mirror animation
            Mirror = false;
            matchBodyPart = AvatarTarget.LeftHand;
        }

        return true;
    }
}
