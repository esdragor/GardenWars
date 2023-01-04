using System.Collections.Generic;
using GameStates;
using Photon.Pun;

namespace Entities.Capacities
{
    public abstract class PassiveCapacity
    {
        protected GameStateMachine gsm => GameStateMachine.Instance;
        
        public byte indexOfSo; //Index Reference in CapacitySOCollectionManager

        public bool stackable;
        public int count; //Amount of Stacks

        public List<Enums.CapacityType> types; //All types of the capacity

        public abstract PassiveCapacitySO AssociatedPassiveCapacitySO();

        protected Entity entity;
        protected Champion.Champion champion => (Champion.Champion) entity;
        

        public void OnAdded(Entity target)
        {
            if (stackable) count++;
            entity = target;
            OnAddedEffects(entity);
        }

        /// <summary>
        /// Call when a Stack of the capicity is Added
        /// </summary>
        protected abstract void OnAddedEffects(Entity target);

        /// <summary>
        /// Call Feedback of the Stack on when Added
        /// </summary>
        public void OnAddedFeedback(Entity target)
        {
            if (stackable && !PhotonNetwork.IsMasterClient) count++;
            entity = target;
            OnAddedFeedbackEffects(target);
        }
        
        protected abstract void OnAddedFeedbackEffects(Entity target);

        /// <summary>
        /// Call when a Stack of the capacity is Removed
        /// </summary>
        public void OnRemoved(Entity target)
        {
            OnRemovedEffects(target);
        }
        
        protected abstract void OnRemovedEffects(Entity target);

        /// <summary>
        /// Call Feedback of the Stack on when Removed
        /// </summary>
        public void OnRemovedFeedback(Entity target)
        {
            OnRemovedFeedbackEffects(target);
        }
        
        protected abstract void OnRemovedFeedbackEffects(Entity target);
    }
}
