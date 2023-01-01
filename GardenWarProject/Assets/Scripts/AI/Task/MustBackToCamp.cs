using System.Collections;
using System.Collections.Generic;
using BehaviourTree;
using UnityEngine;
using UnityEngine.AI;

public class MustBackToCamp : Node
{
    private Node Root;
    private Transform trans;
    private Vector3 Camp;
    private float MaxDist;
    private bool isBack = false;
    private NavMeshAgent agent;
    
    public MustBackToCamp(Node _Root, Transform transform, Vector3 _Camp, float _MaxDist, NavMeshAgent _agent)
    {
        trans = transform;
        Root = _Root;
        Camp = _Camp;
        MaxDist = _MaxDist;
        agent = _agent;
    }


    public override NodeState Evaluate(Node root)
    {
        if (Vector3.Distance(trans.position, Camp) > MaxDist || isBack)
        {
            agent.SetDestination(Camp);
            trans.LookAt(Camp);
            isBack = (!(Vector3.Distance(trans.position, Camp) < 1));
            return NodeState.Success;
        }
        return NodeState.Failure;
    }
}
