using BehaviourTree;
using UnityEngine;

namespace Test.IA_BL.Task
{
    public class CheckCanMove : Node
    {
        private Minion entity;
       
        public CheckCanMove(Minion _entity)
        {
            entity = _entity;
        }
        public override NodeState Evaluate(Node Root)
        {
            return (entity.CanMove()) ? NodeState.Success : NodeState.Failure;
        }
    }
}
