using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Patrol_State_Enemy_Soldier : StateMachineBehaviour
{
    float timer;
    [SerializeField] float patrolTime;

    List<Transform> waypoints = new List<Transform>();

    NavMeshAgent agent;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0;

        //generates a random float to set the time the enemy will be patrolling
        //patrolTime = Random.Range(15.0f, 45.0f);

        //find all the waypoints position within the Waypoints object and adds them to the new waypoints List
        Transform waypointsObject = GameObject.FindGameObjectWithTag("Waypoints").transform;
        foreach (Transform t in waypointsObject)
            waypoints.Add(t);

        agent = animator.GetComponent<NavMeshAgent>();
        agent.SetDestination(waypoints[0].position);

    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if (agent.remainingDistance <= agent.stoppingDistance)
            agent.SetDestination(waypoints[Random.Range(0, waypoints.Count)].position);

        timer += Time.deltaTime;

        if (timer > patrolTime)
            animator.SetBool("isPatrolling", false);

        

    }

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent.SetDestination(agent.transform.position);
        waypoints.Clear();
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
}
