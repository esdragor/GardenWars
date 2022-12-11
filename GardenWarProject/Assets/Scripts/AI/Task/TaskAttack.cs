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
    private GameStateMachine sm => GameStateMachine.Instance;
    private Node Root;
    private byte capacityIndex = 0;
    private Transform _model;


    public TaskAttack(Node _Root, Entity entity, Transform model, byte capaIndex, float _attackSpeed)
    {
        Root = _Root;
        MyEntity = entity;
        attack = entity.GetComponent<IAttackable>();
        attackSpeed = _attackSpeed;
        capacityIndex = capaIndex;
        _model = model;
    }

    public override NodeState Evaluate(Node root)
    {
        if (attack == null) return NodeState.Failure;

        if (!attack.CanAttack()) return NodeState.Failure;

        Entity target = (Entity)root.GetData("target");

        if (target == null) return NodeState.Failure;

        if (PreviousTarget != target)
        {
            PreviousTarget = target;
            CurrentAtkTime = 0f;
        }
        
       //CurrentAtkTime += 1.0f / sm.tickRate;
       CurrentAtkTime += Time.deltaTime;

        if (CurrentAtkTime < attackSpeed) return NodeState.Running;

        CurrentAtkTime = 0f;
        
        attack.RequestAttack(capacityIndex, target.entityIndex, target.position);
        _model.LookAt(target.position);
        root.ClearData("target");

        return NodeState.Success;
    }
}