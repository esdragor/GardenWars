using System.Collections;
using System.Collections.Generic;
using BehaviourTree;
using Test.IA_BL.Task;
using UnityEngine;
using UnityEngine.AI;
using Tree = BehaviourTree.Tree;

public class MyAIBT : Tree
{
    public Transform[] waypoints;
    
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private Minion entity;
    [SerializeField] private float AtkRange;
    [SerializeField] private float FOVRange;

    
    
    protected override Node InitTree()
    {
        Node origin = new Selector(new List<Node>
        {
            new Sequence(new List<Node>
            {
                new CheckEnemyInPOVRange(transform,enemyMask, FOVRange),
                new Selector(new List<Node>
                {
                    new Sequence(new List<Node>
                    {
                        new CheckEnemyInAttackRange(transform, AtkRange),
                        new TaskAttack(entity),
                    }),
                    new Sequence(new List<Node>
                    {
                        new CheckCanMove(entity),
                        new GoToTarget(agent,transform)
                    })
                    
                }),
            }),

            new Sequence(new List<Node>
            {
                new CheckCanMove(entity),
                new TaskPatrol(agent, transform, waypoints, 5f)
            }),


        });

        return origin;
    }
    
}
