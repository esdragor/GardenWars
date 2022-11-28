using System.Collections;
using System.Collections.Generic;
using BehaviourTree;
using UnityEngine;
using Tree = BehaviourTree.Tree;

public class MyAIBT : Tree
{
    public UnityEngine.Transform[] waypoints;
    public static float speed = 4f;

    [SerializeField] private LayerMask enemyMask;

    protected override Node InitTree()
    {
        Node origin = new Selector(new List<Node>
        {
            new Sequence(new List<Node>
            {
                new CheckEnemyInPOVRange(transform, enemyMask,LayerMask.GetMask("Enemy"), 20f),
                new GoToTarget(transform)
            }),
            new TaskPatrol(transform, waypoints, 5f)

        });

        return origin;
    }
}
