using System.Collections;
using System.Collections.Generic;
using BehaviourTree;
using Entities;
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
    private IActiveLifeable lifeable;
    private Entity entity;
    
    public MustBackToCamp(Node _Root, Transform transform, Vector3 _Camp, float _MaxDist, NavMeshAgent _agent, IActiveLifeable _lifeable)
    {
        trans = transform;
        Root = _Root;
        Camp = new Vector3(_Camp.x, transform.position.y, _Camp.z);
        MaxDist = _MaxDist;
        agent = _agent;
        lifeable = _lifeable;
        entity = transform.GetComponent<Entity>();
    }


    public override NodeState Evaluate(Node root)
    {
        if (!isBack)
            entity.SetAnimatorTrigger("Idle");
        if (Vector3.Distance(trans.position, Camp) > MaxDist || isBack)
        {
            //lifeable.SetCurrentHpRPC(lifeable.GetMaxHp());
            agent.SetDestination(Camp);
            trans.LookAt(Camp);
            isBack = (!(Vector3.Distance(trans.position, Camp) <= 2));

            return NodeState.Success;
        }
        return NodeState.Failure;
    }
}
