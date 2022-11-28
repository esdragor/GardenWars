using BehaviourTree;
using UnityEngine;

public class GoToTarget : Node
{
    private Transform MyTransform;

    public GoToTarget(Transform trans)
    {
        MyTransform = trans;
    }

    public override NodeState Evaluate()
    {
        Transform target = (Transform)Parent.GetData("target");

        float TEST = Vector3.Distance(MyTransform.position, target.position);
        
        if (Vector3.Distance(MyTransform.position, target.position) > 1f)
        {
            MyTransform.position =
                Vector3.MoveTowards(MyTransform.position, target.position, MyAIBT.speed * Time.deltaTime);
            MyTransform.LookAt(target.position);
        }

        state = NodeState.Running;
        return state;
    }
}
