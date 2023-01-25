using System.Collections;
using System.Collections.Generic;
using BehaviourTree;
using FMODUnity;
using UnityEngine;

public class IsLastCheckpoint : Node
{
    private Minion m;
    private int l;
    
    public IsLastCheckpoint(Minion minion, int length)
    {
        m = minion;
        l = length;
    }

    public override NodeState Evaluate(Node root)
    {
        if (m.lastCheckpoint == l - 1)
        {
            return NodeState.Failure;
        }
        else
        {
            return NodeState.Success;
        }
    }
}
