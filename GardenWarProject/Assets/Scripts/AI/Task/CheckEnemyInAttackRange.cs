using System.Collections;
using System.Collections.Generic;
using BehaviourTree;
using Entities;
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
        Entity target = (Entity)Parent.Parent.GetData("target");
        
        if (target == null) return NodeState.Failure;
        
       if (Vector3.Distance(trans.position, target.transform.position) < atkRange)
           return NodeState.Success;

       return NodeState.Failure;
    }
}
