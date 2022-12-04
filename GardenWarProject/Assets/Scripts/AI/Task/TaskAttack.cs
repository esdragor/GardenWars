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
    private float CurrentAtkTime = 0f;
    private Entity PreviousTarget;
    private GameStateMachine sm = null;

    public TaskAttack(Minion entity)
    {
        attack = entity.gameObject.GetComponent<IAttackable>();
        attackSpeed = entity.GetAttackSpeed();
    }

    public override NodeState Evaluate()
    {        
        if (attack == null) return NodeState.Failure;

        if (!attack.CanAttack()) return NodeState.Failure;

        Entity target = (Entity)Parent.Parent.GetData("target");

        if (target == null) return NodeState.Failure;
        
        if (PreviousTarget != target)
        {
            PreviousTarget = target;
            CurrentAtkTime = 0f;
        }

        if (!sm)
            sm = GameStateMachine.Instance;
        
        CurrentAtkTime += 1.0f / (float)sm.tickRate;

        if (CurrentAtkTime < attackSpeed) return NodeState.Running;

        CurrentAtkTime = 0f;
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