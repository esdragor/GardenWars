using System.Collections;
using System.Collections.Generic;
using BehaviourTree;
using GameStates;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.AI;

public class TaskPatrol : Node
{
    private Transform Mytransform;
    private Transform model;

    private Transform[] waypoints;

    //private Animator animator;
    private int CurrentWaypointIndex = 0;
    private float waitTime = 1f;
    private float waitCounter = 0f;
    private bool waiting = false;
    private NavMeshAgent agent;
    private Minion minion;

    public TaskPatrol(NavMeshAgent _agent, Minion _minion, Transform _transform, Transform _model, Transform[] _waypoints,
        float waitingBetweenTwoPoints = 1f)
    {
        Mytransform = _transform;
        waypoints = _waypoints;
        waitTime = waitingBetweenTwoPoints;
        agent = _agent;
        minion = _minion;
        //animator = getComponent<Animator>();
        model = _model;
    }

    public override NodeState Evaluate(Node Root)
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
            Vector3 MyPos = Mytransform.position;
            


            if (CurrentWaypointIndex + 1 < waypoints.Length)
            {
                if(MyPos.x > waypoints[CurrentWaypointIndex].position.x && MyPos.x < waypoints[CurrentWaypointIndex + 1].position.x)
                {
                    CurrentWaypointIndex++;
                }
                else if(MyPos.x < waypoints[CurrentWaypointIndex].position.x && MyPos.x > waypoints[CurrentWaypointIndex + 1].position.x)
                {
                    CurrentWaypointIndex++;
                }
            }
           if (CurrentWaypointIndex >= waypoints.Length) // agent has reached the last waypoint
            {
                minion.ReachEnemyCamp();
                return NodeState.Running; 
            }
           
           Vector3 pos = waypoints[CurrentWaypointIndex].position;
           pos.y = 1.5f;
            
            if (Vector3.Distance(MyPos, pos) < 0.3f)
            {
                waitCounter = 0f;
                waiting = true;
                CurrentWaypointIndex++;
                //animator.SetBool("Walking", false);
            }
            agent.SetDestination(pos);
        }

        state = NodeState.Running;
        return state;
    }
}