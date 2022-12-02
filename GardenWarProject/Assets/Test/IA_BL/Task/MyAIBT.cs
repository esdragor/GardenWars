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

    
    
    protected override Node InitTree()
    {
        Node origin = new Selector(new List<Node>
        {
            new Sequence(new List<Node>
            {
                new CheckCanMove(entity),
                new CheckEnemyInPOVRange(transform, enemyMask,LayerMask.GetMask("Enemy"), 20f),
                new GoToTarget(agent,transform)
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
