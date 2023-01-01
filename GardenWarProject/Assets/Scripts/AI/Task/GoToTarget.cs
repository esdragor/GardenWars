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
    private Transform model;
    private NavMeshAgent agent;
    private Node Root;

    public GoToTarget(Node _Root, NavMeshAgent _agent, Transform trans, Transform _model)
    {
        MyTransform = trans;
        agent = _agent;
        Root = _Root;
        model = _model;
    }

    public override NodeState Evaluate(Node Root)
    {
        Vector3 pos = ((Entity)Root.GetData("target")).transform.position;
        Vector3 MyPos = MyTransform.position;
        
        pos.y = 1.5f;
        
        if (Vector3.Distance(MyPos, pos) > agent.radius)
        {
            agent.SetDestination(pos);
            Debug.Log(Vector3.Distance(MyPos, pos));
                //Vector3.MoveTowards(MyPos, pos, agent.speed * Time.deltaTime));
            MyTransform.LookAt(model);
            state = NodeState.Running;
            return state;
        }
        
        state = NodeState.Success;
        return state;
    }
}
