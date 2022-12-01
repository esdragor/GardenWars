using System.Numerics;
using BehaviourTree;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using Vector3 = UnityEngine.Vector3;

public class GoToTarget : Node
{
    private Transform MyTransform;
    private NavMeshAgent agent;

    public GoToTarget(NavMeshAgent _agent, Transform trans)
    {
        MyTransform = trans;
        agent = _agent;
    }

    public override NodeState Evaluate()
    {
        Vector3 pos = ((Transform)Parent.GetData("target")).position;
        Vector3 MyPos = MyTransform.position;

        float TEST = Vector3.Distance(MyPos, pos);
        
        if (Vector3.Distance(MyTransform.position, pos) > 2f)
        {
            agent.SetDestination(
                Vector3.MoveTowards(MyPos, pos, MyAIBT.speed * Time.deltaTime));
            MyTransform.LookAt(pos);
        }

        state = NodeState.Running;
        return state;
    }
}
