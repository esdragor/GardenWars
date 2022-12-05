using System.Numerics;
using BehaviourTree;
using Entities;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using Vector3 = UnityEngine.Vector3;

public class GoToTarget : Node
{
    private Transform MyTransform;
    private NavMeshAgent agent;
    private Node Root;

    public GoToTarget(Node _Root, NavMeshAgent _agent, Transform trans)
    {
        MyTransform = trans;
        agent = _agent;
        Root = _Root;
    }

    public override NodeState Evaluate()
    {
        Vector3 pos = ((Entity)Root.GetData("target")).transform.position;
        Vector3 MyPos = MyTransform.position;
        
        pos.y = 1.5f;
        
        if (Vector3.Distance(MyTransform.position, pos) > 1f)
        {
            agent.SetDestination(
                Vector3.MoveTowards(MyPos, pos, agent.speed * Time.deltaTime));
            MyTransform.LookAt(pos);
            state = NodeState.Running;
            return state;
        }
        
        state = NodeState.Success;
        return state;
    }
}
