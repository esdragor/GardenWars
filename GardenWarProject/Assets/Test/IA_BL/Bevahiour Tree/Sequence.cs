using System.Collections.Generic;


namespace BehaviourTree
{
    public class Sequence : Node
    {
        public Sequence() : base()
        {
        }

        public Sequence(List<Node> children) : base(children)
        {
        }

        public override NodeState Evaluate()
        {
            bool ChildRunning = false;

            foreach (Node node in children)
            {
                switch (node.Evaluate())
                {
                    case NodeState.Failure:
                        state = NodeState.Failure;
                        return state;
                    case NodeState.Success:
                        continue;
                    case NodeState.Running:
                        ChildRunning = true;
                        continue;
                    default:
                        state = NodeState.Success;
                        return state;
                }
            }

            state = (ChildRunning) ? NodeState.Running : NodeState.Success;
            return state;
        }
    }
}