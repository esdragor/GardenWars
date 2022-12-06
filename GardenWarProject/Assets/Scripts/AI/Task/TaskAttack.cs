using System.Collections;
using System.Collections.Generic;
using BehaviourTree;
using Entities;
using Entities.Capacities;
using Entities.Champion;
using GameStates;
using JetBrains.Annotations;
using UnityEngine;

public class TaskAttack : Node
{
    private IAttackable attack;
    private Entity MyEntity;
    private float attackSpeed;
    private double CurrentAtkTime = 0f;
    private Entity PreviousTarget;
    private GameStateMachine sm = null;
    private Node Root;
    private byte capacityIndex = 0;


    public TaskAttack(Node _Root, Entity entity, byte capaIndex, float _attackSpeed)
    {
        Root = _Root;
        MyEntity = entity;
        attack = entity.GetComponent<IAttackable>();
        attackSpeed = _attackSpeed;
        capacityIndex = capaIndex;
    }

    public override NodeState Evaluate(Node Root)
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

       //CurrentAtkTime += 1.0f / sm.tickRate;
       CurrentAtkTime += Time.deltaTime;

        if (CurrentAtkTime < attackSpeed) return NodeState.Running;

        CurrentAtkTime = 0f;
        
        attack.RequestAttack(capacityIndex, new[] { target.entityIndex }, new[] { target.transform.position });
        MyEntity.transform.LookAt(target.transform.position);
        Root.ClearData("target");

        return NodeState.Success;
    }
}