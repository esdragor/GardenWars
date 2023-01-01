using System.Collections;
using System.Collections.Generic;
using BehaviourTree;
using Test.IA_BL.Task;
using UnityEngine;
using UnityEngine.AI;
using Tree = BehaviourTree.Tree;

public class PinataBT : Tree
{
    public Transform CampPosition;
    public float MaxDistanceToCamp;
    
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private Pinata entity;
    [SerializeField] private float AtkRange;
    [SerializeField] private float FOVRange;
    [SerializeField] private float atkDelay = 15;
    [SerializeField] private Transform Model;

    private Node origin;
    
    protected override Node InitTree()
    {
        origin = new Selector(new List<Node>
        {
            new Sequence(new List<Node>
            {
                new CheckCanMove(entity),
                new MustBackToCamp(origin, transform, CampPosition.position, MaxDistanceToCamp, agent)
            }),
            new Sequence(new List<Node>
            {
                new CheckEnemyInPOVRange(origin, entity,enemyMask, FOVRange),
                new Selector(new List<Node>
                {
                    new Sequence(new List<Node>
                    {
                        new CheckEnemyInAttackRange(origin, transform, AtkRange),
                        new TaskAttack(origin, entity, Model, entity.activePinataAutoSO.indexInCollection, atkDelay),
                    }),
                    new Sequence(new List<Node>
                    {
                        new CheckCanMove(entity),
                        new GoToTarget(origin, agent, transform, Model)
                    })
                }),
            }),


        });

        return origin;
    }

}
