using System.Collections;
using System.Collections.Generic;
using BehaviourTree;
using UnityEngine;

public class CheckEnemyInAttackRange : Node
{
    private Transform trans;
    private float atkRange;
    
    public CheckEnemyInAttackRange(Transform transform, float AttackRange)
    {
        trans = transform;
        atkRange = AttackRange;
    }
    public override NodeState Evaluate()
    {
        object t = Parent.Parent.GetData("target");
        
        if (t == null) return NodeState.Failure;

       Transform target = (Transform)t;
       if (Vector3.Distance(trans.position, target.position) < atkRange)
           return NodeState.Success;

       return NodeState.Failure;
    }
}
