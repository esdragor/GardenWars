using BehaviourTree;
using Entities;
using UnityEngine;

namespace Test.IA_BL.Task
{
    public class CheckCanMove : Node
    {
        private IMoveable entity;
       
        public CheckCanMove(IMoveable _entity)
        {
            entity = _entity;
        }
        public override NodeState Evaluate(Node Root)
        {
            return (entity.CanMove()) ? NodeState.Success : NodeState.Failure;
        }
    }
}
