using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopActionControl : StateMachineBehaviour
{
    PlayerMovement player;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player == null)
            player = animator.GetComponent<PlayerMovement>();

       player.HasControl = false;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.HasControl = true;
    }
}
