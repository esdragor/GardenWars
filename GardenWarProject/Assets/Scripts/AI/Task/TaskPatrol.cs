using System.Collections;
using System.Collections.Generic;
using BehaviourTree;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.AI;

public class TaskPatrol : Node
{
    private Transform Mytransform;

    private Transform[] waypoints;

    //private Animator animator;
    private int CurrentWaypointIndex = 0;
    private float waitTime = 1f;
    private float waitCounter = 0f;
    private bool waiting = false;
    private NavMeshAgent agent;
    private Minion minion;

    public TaskPatrol(NavMeshAgent _agent, Minion _minion, Transform _transform, Transform[] _waypoints,
        float waitingBetweenTwoPoints = 1f)
    {
        Mytransform = _transform;
        waypoints = _waypoints;
        waitTime = waitingBetweenTwoPoints;
        agent = _agent;
        minion = _minion;
        //animator = getComponent<Animator>();
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
            Vector3 pos = waypoints[CurrentWaypointIndex].position;
            Vector3 MyPos = Mytransform.position;
            
            pos.y = 1.5f;

            Vector3 direction = Vector3.MoveTowards(MyPos, pos, agent.speed * Time.deltaTime);

            if (Vector3.Distance(Mytransform.position, pos) < 0.3f)
            {
                agent.SetDestination(direction);
                waitCounter = 0f;
                waiting = true;
                if (CurrentWaypointIndex + 1 >= waypoints.Length)// agent has reached the last waypoint
                {
                    minion.ReachEnemyCamp();
                    return NodeState.Running; 
                }
                CurrentWaypointIndex++;
                //animator.SetBool("Walking", false);
            }
            else
            {
                agent.SetDestination(direction);
                Mytransform.LookAt(pos);
            }
        }

        state = NodeState.Running;
        return state;
    }
}