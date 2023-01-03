using System.Collections;
using System.Collections.Generic;
using BehaviourTree;
using Entities;
using Entities.Capacities;
using Entities.Champion;
using GameStates;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

public class TaskAttack : Node
{
    private IAttackable attackable;
    private Entity MyEntity;
    private float attackSpeed;
    private double CurrentAtkTime = 0f;
    private Entity PreviousTarget;
    private GameStateMachine sm => GameStateMachine.Instance;
    private Node Root;
    private byte capacityIndex = 0;
    private Transform model;
    private NavMeshAgent agent;


    public TaskAttack(Node _Root, Entity entity, Transform _model, byte capaIndex, float _attackSpeed, NavMeshAgent _agent)
    {
        Root = _Root;
        MyEntity = entity;
        attackable = entity.GetComponent<IAttackable>();
        attackSpeed = _attackSpeed;
        capacityIndex = capaIndex;
        model = _model;
        agent = _agent;
    }

    public override NodeState Evaluate(Node root)
    {
        if (attackable == null) return NodeState.Failure;

        if (!attackable.CanAttack()) return NodeState.Failure;

        Entity target = (Entity)root.GetData("target");

        if (target == null) return NodeState.Failure;

        if (PreviousTarget != target)
        {
            PreviousTarget = target;
            CurrentAtkTime = 0f;
        }
        
       //CurrentAtkTime += 1.0f / sm.tickRate;
       CurrentAtkTime += Time.deltaTime;
        model.LookAt(new Vector3(target.position.x, model.position.y, target.position.z));
        
        if (agent)
        agent.SetDestination(MyEntity.transform.position);

        if (CurrentAtkTime < attackSpeed) return NodeState.Running;

        CurrentAtkTime = 0f;
        
        attackable.RequestAttack(capacityIndex, target.entityIndex, target.position);
        root.ClearData("target");
        
        if(MyEntity is Pinata)
        {
            Debug.Log("A SUPPRIMER");
            MyEntity.GetComponent<IActiveLifeable>().DecreaseCurrentHpRPC(1000, target.entityIndex);
        }
        
        return NodeState.Success;
    }
}