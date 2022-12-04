using System.Collections;
using System.Collections.Generic;
using BehaviourTree;
using Entities;
using UnityEngine;

public class CheckEnemyInAttackRange : Node
{
    private Transform trans;
    private float atkRange;
    private Node Root;
    
    public CheckEnemyInAttackRange(Node _Root, Transform transform, float AttackRange)
    {
        trans = transform;
        atkRange = AttackRange;
        Root = _Root;
    }
    public override NodeState Evaluate()
    {
        Entity target = (Entity)Root.GetData("target");
        
        if (target == null) return NodeState.Failure;
        
       if (Vector3.Distance(trans.position, target.transform.position) < atkRange)
           return NodeState.Success;

       return NodeState.Failure;
    }
}
