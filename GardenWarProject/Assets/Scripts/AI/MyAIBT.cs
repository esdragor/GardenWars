using System.Collections;
using System.Collections.Generic;
using BehaviourTree;
using Test.IA_BL.Task;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using Tree = BehaviourTree.Tree;

public class MyAIBT : Tree
{
    public Transform[] waypoints;
    
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private Minion entity;
    [SerializeField] private float AtkRange;
    [SerializeField] private float FOVRange;
    [SerializeField] private float atkDelay = 15;

    private Node origin;
    
    protected override Node InitTree()
    {
        origin = new Selector(new List<Node>
        {
            new Sequence(new List<Node>
            {
                new CheckEnemyInPOVRange(origin, entity,enemyMask, FOVRange),
                new Selector(new List<Node>
                {
                    new Sequence(new List<Node>
                    {
                        new CheckEnemyInAttackRange(origin, transform, AtkRange),
                        new TaskAttack(origin, entity, entity.activeMinionAutoSO.indexInCollection, atkDelay),
                    }),
                    new Sequence(new List<Node>
                    {
                        new CheckCanMove(entity),
                        new GoToTarget(origin, agent, transform)
                    })
                    
                }),
            }),

            new Sequence(new List<Node>
            {
                new CheckCanMove(entity),
                new TaskPatrol(agent, entity, transform, waypoints, 5f)
            }),


        });

        return origin;
    }
    
}
