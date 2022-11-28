using System.Collections;
using System.Collections.Generic;
using BehaviourTree;
using UnityEditor.UIElements;
using UnityEngine;

public class TaskPatrol : Node
{
    private Transform Mytransform;
    private Transform[] waypoints;
    //private Animator animator;
    private int CurrentWaypointIndex = 0;
    private float waitTime = 1f;
    private float waitCounter = 0f;
    private bool waiting = false;
    
    public TaskPatrol(Transform _transform, Transform[] _waypoints, float waitingBetweenTwoPoints = 1f)
    {
        Mytransform = _transform;
        waypoints = _waypoints;
        waitTime = waitingBetweenTwoPoints;
        //animator = getComponent<Animator>();
    }

    public override NodeState Evaluate()
    {
        if (waiting)
        {
            waitCounter += Time.deltaTime;
            if (waitCounter < waitTime)
            {
                waiting = false;
                //animator.SetBool("Walking", true);
            }
        }
        else
        {
            Transform trans = waypoints[CurrentWaypointIndex];
            if (Vector3.Distance(Mytransform.position, trans.position) < 0.1f)
            {
                Mytransform.position = trans.position;
                waitCounter = 0f;
                waiting = true;

                CurrentWaypointIndex = (CurrentWaypointIndex + 1) % waypoints.Length;
                //animator.SetBool("Walking", false);
            }
            else
            {
                Mytransform.position =
                    Vector3.MoveTowards(Mytransform.position, trans.position, MyAIBT.speed * Time.deltaTime);
                Mytransform.LookAt(trans.position);
            }
        }

        state = NodeState.Running;
        return state;
    }
}
