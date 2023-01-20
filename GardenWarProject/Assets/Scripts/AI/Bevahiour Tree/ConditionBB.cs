using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public enum Condition
    {
        Superior,
        Inferior,
        Equal,
        Different,
    }
    
    public class ConditionBB : Node
    {
        private string type;
        private string key;
        Condition condition;

        public ConditionBB(string _type, Condition cond, string _valueName)
        {
            type = _type;
            key = _valueName;
            condition = cond;
        }

        public override NodeState Evaluate(Node root)
        {
            switch (type)
            {
                case "int":
                    break;
                case "float":
                    break;
                case "string":
                    break;
                case "bool":
                    bool test = (bool)root.GetData(key);
                    if ((!test && condition == Condition.Different) || (test && condition == Condition.Equal))
                    {
                        return NodeState.Success;
                    }
                    return NodeState.Failure;
                default:
                    return NodeState.Failure;
            }
            return NodeState.Failure;
        }
    }
}