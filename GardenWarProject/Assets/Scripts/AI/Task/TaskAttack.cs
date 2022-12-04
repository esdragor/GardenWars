using System.Collections;
using System.Collections.Generic;
using BehaviourTree;
using Entities;
using Entities.Champion;
using UnityEngine;

public class TaskAttack : Node
{
    private IAttackable attack;
    private float attackSpeed;
    private float CurrentAtkTime = 0f;
    private string PreviousTargetName = "";
    
    public TaskAttack(Minion entity)
    {
        attack = entity.gameObject.GetComponent<IAttackable>();
        attackSpeed = entity.GetAttackSpeed();
    }
    
    public override NodeState Evaluate()
    {
        if (attack == null) return NodeState.Failure;
        
        if (!attack.CanAttack())  return NodeState.Failure;
        
        object t = Parent.Parent.GetData("target");
        
        if (t == null) return NodeState.Failure;
        
        Transform target = (Transform)t;
        
        if (PreviousTargetName != target.name)
        {
            PreviousTargetName = target.name;
            CurrentAtkTime = 0f;
        }
        
        CurrentAtkTime += Time.deltaTime;
        
        if (CurrentAtkTime < attackSpeed) return NodeState.Running;

        Minion minionEntity = target.gameObject.GetComponent<Minion>();

        if (minionEntity)
        {
           Debug.Log("Attack Minion");
        }
        
        Champion herosEntity = target.gameObject.GetComponent<Champion>();

        if (herosEntity)
        {
            Debug.Log("Attack Hero");
        }
        
        Parent.Parent.ClearData("target");
        
        return NodeState.Success;
    }
}
