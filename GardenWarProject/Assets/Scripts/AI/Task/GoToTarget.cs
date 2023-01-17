using System.Numerics;
using BehaviourTree;
using Entities;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using Tree = UnityEngine.Tree;
using Vector3 = UnityEngine.Vector3;

public class GoToTarget : Node
{
    private Transform MyTransform;
    private NavMeshAgent agent;
    private Entity entity;

    public GoToTarget(NavMeshAgent _agent, Transform trans)
    {
        MyTransform = trans;
        agent = _agent;
    }

    public override NodeState Evaluate(Node Root)
    {
        Entity t = ((Entity)Root.GetData("target"));
        
        if (t == null) return NodeState.Failure;
        
        Vector3 pos = t.transform.position;
        Vector3 MyPos = MyTransform.position;
        
        pos.y = 1.5f;
        
        if (Vector3.Distance(MyPos, pos) > agent.radius)
        {
            agent.SetDestination(pos);
            //Vector3.MoveTowards(MyPos, pos, agent.speed * Time.deltaTime));
            //MyTransform.LookAt(model);
            
            if (!entity)
                entity = MyTransform.GetComponent<Entity>();
            entity.SetAnimatorTrigger("Move");
            state = NodeState.Running;
            return state;
        }
        
        state = NodeState.Success;
        return state;
    }
}
