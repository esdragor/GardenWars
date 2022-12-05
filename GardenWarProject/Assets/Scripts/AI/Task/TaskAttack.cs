using System.Collections;
using System.Collections.Generic;
using BehaviourTree;
using Entities;
using Entities.Champion;
using GameStates;
using UnityEngine;

public class TaskAttack : Node
{
    private IAttackable attack;
    private float attackSpeed;
    private double CurrentAtkTime = 0f;
    private Entity PreviousTarget;
    private GameStateMachine sm = null;
    private Node Root;
    private bool IsTower = false;
    
    public TaskAttack(Node _Root, Entity entity, float _attackSpeed, bool _IsTower = false)
    {
        Root = _Root;
        attack = entity.GetComponent<IAttackable>();
        attackSpeed = _attackSpeed;
        IsTower = _IsTower;
    }

    public override NodeState Evaluate()
    {        
        if (attack == null) return NodeState.Failure;

        if (!attack.CanAttack()) return NodeState.Failure;

        Entity target = (Entity)Root.GetData("target");

        if (target == null) return NodeState.Failure;
        
        if (PreviousTarget != target)
        {
            PreviousTarget = target;
            CurrentAtkTime = 0f;
        }

        if (!sm)
            sm = GameStateMachine.Instance;
        
        CurrentAtkTime += 1.0f / sm.tickRate;

        if (CurrentAtkTime < attackSpeed) return NodeState.Running;

        CurrentAtkTime = 0f;
        
        if (IsTower)
            Debug.Log("Tower Attack");
        else
        {
            Debug.Log("Minion Attack");
        }
        
        attack.RequestAttack(0, new []{ target.entityIndex}, new []{target.transform.position});

        Root.ClearData("target");

        return NodeState.Success;
    }
}