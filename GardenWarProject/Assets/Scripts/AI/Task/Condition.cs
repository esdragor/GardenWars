using System.Collections;
using System.Collections.Generic;
using BehaviourTree;
using UnityEngine;

public class Condition<T> : Node
{
    private T type;
    
    public Condition(T type)
    {
        this.type = type;
    }

    public override NodeState Evaluate(Node root)
    {
        T value = (T)root.GetData("target");

        return NodeState.Success;
    }
}
