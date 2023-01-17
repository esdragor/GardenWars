using System.Collections;
using System.Collections.Generic;
using BehaviourTree;
using Entities;
using UnityEngine;
using Tree = BehaviourTree.Tree;

public class CheckEnemyInAttackRange : Node
{
    private Transform trans;
    private float atkRange;
    private Tree Root;
    
    public CheckEnemyInAttackRange(Tree _Root, Transform transform, float AttackRange)
    {
        trans = transform;
        atkRange = AttackRange;
        Root = _Root;
    }
    public override NodeState Evaluate(Node _Root)
    {
        Entity target = (Entity)Root.getOrigin().GetData("target");
        
        if (target == null) return NodeState.Failure;
        
       if (Vector3.Distance(trans.position, new Vector3(target.transform.position.x, trans.position.y, target.transform.position.z)) < atkRange)
           return NodeState.Success;

       return NodeState.Failure;
    }
}
