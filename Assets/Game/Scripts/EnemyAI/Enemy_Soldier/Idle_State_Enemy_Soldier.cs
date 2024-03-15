using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle_State_Enemy_Soldier : StateMachineBehaviour
{

    float timer;

    [SerializeField] AnimationClip[] idleAnimations;
    int idleAnimIndex;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        PlayAnimation(animator);


    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer += Time.deltaTime;

        if (timer > 10)
            animator.SetBool("isPatrolling", true);


    }

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
 

    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}


    void PlayAnimation(Animator animator) 
    {

        timer = 0;

        idleAnimIndex = Random.Range(0, idleAnimations.Length);
        animator.SetInteger("IdleAnimIndex", idleAnimIndex);
        //animator.CrossFade(idleAnimations[idleAnimIndex].name, 0f); //  <<<< weird behaviour 
        animator.Play(idleAnimIndex, 0, 0f);

        if (idleAnimations[idleAnimIndex].name == "IDLE3")
            Debug.Log("YAWN");

    }

}
