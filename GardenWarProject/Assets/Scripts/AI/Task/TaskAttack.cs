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
using Tree = BehaviourTree.Tree;

public class TaskAttack : Node
{
    private IAttackable attackable;
    private Entity MyEntity;
    private float attackSpeed;
    private double CurrentAtkTime = 0f;
    private Entity PreviousTarget;
    private Vector3 Previouspos = Vector3.zero * 999;
    private Tree Root;
    private byte capacityIndex = 0;
    private Transform model;
    private NavMeshAgent agent;
    private string soundAttack = null;



    public TaskAttack(Tree _Root, Entity entity, Transform _model, byte capaIndex, float _attackSpeed,
        NavMeshAgent _agent, string _soundAttack = null)
    {
        Root = _Root;
        MyEntity = entity;
        attackable = entity.GetComponent<IAttackable>();
        attackSpeed = _attackSpeed;
        capacityIndex = capaIndex;
        model = _model;
        agent = _agent;
        soundAttack = _soundAttack;
    }

    public override NodeState Evaluate(Node root)
    {
        if (attackable == null) return NodeState.Failure;

        if (!attackable.CanAttack()) return NodeState.Failure;

        Entity target = (Entity)root.GetData("target");

        if (target == null) return NodeState.Failure;

        if (target.gameObject.activeSelf == false)
        {
            Root.getOrigin().ClearData("target");
            return NodeState.Failure;
        }

        if (PreviousTarget != target)
        {
            PreviousTarget = target;
            CurrentAtkTime = attackSpeed;
        }
        
        CurrentAtkTime += Time.deltaTime;
        if (Previouspos != target.transform.position)
        {
            model.LookAt(new Vector3(target.position.x, model.position.y, target.position.z));
            if(MyEntity is Tower) MyEntity.SetAnimatorTrigger("Turn");
            Previouspos = target.transform.position;
        }
        else
        {
            if(MyEntity is Tower) MyEntity.SetAnimatorTrigger("TurnOff");
        }


        if (agent) agent.SetDestination(MyEntity.transform.position);

        if (CurrentAtkTime < attackSpeed) return NodeState.Running;

        CurrentAtkTime = 0f;
        
        if (soundAttack != null)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/" + soundAttack,model.position);
        }
        attackable.RequestAttack(capacityIndex, target.entityIndex, target.position);
        
        MyEntity.SetAnimatorTrigger("Fire");

        return NodeState.Success;
    }
}