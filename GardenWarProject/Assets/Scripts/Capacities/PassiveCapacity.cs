using System;
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

        public double duration => AssociatedPassiveCapacitySO().duration;
        public double internalPassiveTimer;
        public bool isOnCooldown { get; private set; }

        private bool subscribedForTimer = false;

        public PassiveCapacitySO AssociatedPassiveCapacitySO()
        {
            return CapacitySOCollectionManager.Instance.GetPassiveCapacitySOByIndex(indexOfSo);
        }

        protected Entity entity;
        protected Champion.Champion champion => (Champion.Champion) entity;
        
        public void OnAdded(Entity target)
        {
            if (stackable) count++;
            
            entity = target;
            
            OnAddedEffects(entity);
            
            OnAddedEffectsCallback?.Invoke(entity);
        }

        /// <summary>
        /// Called when a Stack of the capacity is Added
        /// </summary>
        protected abstract void OnAddedEffects(Entity target);

        /// <summary>
        /// Call Feedback of the Stack on when Added
        /// </summary>
        public void OnAddedFeedback(Entity target)
        {
            if (stackable && !PhotonNetwork.IsMasterClient) count++;

            if (internalPassiveTimer > 0)
            {
                SetupTimer(internalPassiveTimer);
            }   
            
            entity = target;
            
            OnAddedFeedbackEffects(entity);

            if (entity.isLocal)
            {
                OnAddedLocalEffects(entity);
                OnAddedLocalEffectsCallback?.Invoke(entity);
            }
            
            OnAddedEffectsFeedbackCallback?.Invoke(entity);
        }

        private void SetupTimer(double time)
        {
            internalPassiveTimer = time;
            isOnCooldown = true;

            gsm.OnTickFeedback += DecreaseTimer;
            subscribedForTimer = true;
        }
        
        private void DecreaseTimer()
        {
            internalPassiveTimer -= 1 / gsm.tickRate;

            if (!(internalPassiveTimer <= 0)) return;
                
            gsm.OnTickFeedback -= DecreaseTimer;
            subscribedForTimer = false;
            isOnCooldown = false;
                
            if (Entity.isMaster)
            {
                entity.RemovePassiveCapacityByIndexRPC(indexOfSo);
            }
        }

        protected abstract void OnAddedFeedbackEffects(Entity target);
        protected abstract void OnAddedLocalEffects(Entity target);

        public event Action<Entity> OnAddedEffectsCallback;
        public event Action<Entity> OnAddedEffectsFeedbackCallback;
        public event Action<Entity> OnAddedLocalEffectsCallback;
        

        /// <summary>
        /// Call when a Stack of the capacity is Removed
        /// </summary>
        public void OnRemoved(Entity target)
        {
            if (stackable) count--;
            OnRemovedEffects(target);
            OnRemovedEffectsCallback?.Invoke(target);
        }
        
        protected abstract void OnRemovedEffects(Entity target);

        /// <summary>
        /// Call Feedback of the Stack on when Removed
        /// </summary>
        public void OnRemovedFeedback(Entity target)
        {
            if (stackable && !PhotonNetwork.IsMasterClient) count--;
            if (subscribedForTimer)
            {
                gsm.OnTickFeedback -= DecreaseTimer;
                subscribedForTimer = false;
                isOnCooldown = false;
            }
            OnRemovedFeedbackEffects(target);
            if (entity.isLocal)
            {
                OnRemovedLocalEffects(entity);
                OnRemovedLocalEffectsCallback?.Invoke(target);
            }
            OnRemovedEffectsFeedbackCallback?.Invoke(target);
        }
        
        protected abstract void OnRemovedFeedbackEffects(Entity target);
        protected abstract void OnRemovedLocalEffects(Entity target);
        
        public event Action<Entity> OnRemovedEffectsCallback;
        public event Action<Entity> OnRemovedEffectsFeedbackCallback;
        public event Action<Entity> OnRemovedLocalEffectsCallback;
    }
}
