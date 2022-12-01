using System.Collections;
using System.Collections.Generic;
using BehaviourTree;
using UnityEngine;
using UnityEngine.AI;
using Tree = BehaviourTree.Tree;

public class MyAIBT : Tree
{
    public Transform[] waypoints;
    
    [SerializeField] private NavMeshAgent agent;
    
    public static  float speed = 1f;

    [SerializeField] private LayerMask enemyMask;

    protected override Node InitTree()
    {
        Node origin = new Selector(new List<Node>
        {
            new Sequence(new List<Node>
            {
                new CheckEnemyInPOVRange(transform, enemyMask,LayerMask.GetMask("Enemy"), 20f),
                new GoToTarget(agent, transform)
            }),
            new TaskPatrol(agent, transform, waypoints, 5f)

        });

        return origin;
    }
    
}
