using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class Selector : Node
    {
        public Selector() : base() {}

        public Selector(List<Node> children) : base(children)
        {
        }

        public override NodeState Evaluate(Node Root)
        {
            foreach (Node node in children)
            {
                switch (node.Evaluate(Root))
                {
                    case NodeState.Failure:
                        continue;
                    case NodeState.Success:
                        return state;
                    case NodeState.Running:
                        state = NodeState.Running;
                        return state;
                    default:
                        continue;
                }
            }

            state = NodeState.Failure;
            return state;
        }
    }
}
