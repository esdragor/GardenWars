using System.Collections.Generic;
using Entities.Capacities;
using Photon.Pun;
using UnityEngine;

namespace Entities.Inventory
{
    [System.Serializable]
    public abstract class Item
    {
        public static bool isOffline => !PhotonNetwork.IsConnected;
        public static bool isMaster => isOffline || PhotonNetwork.IsMasterClient;

        public bool consumable;
        public int count;

        public Entity entityOfInventory;
        public IInventoryable inventory;

        public double baseCooldown => AssociatedItemSO().activationCooldown;
        public bool isOnCooldown;
        private double cooldownTimer;
        public readonly List<ActiveCapacity> activeCapacities = new List<ActiveCapacity>();
        
        public Sprite sprite => AssociatedItemSO().sprite;

        public byte indexOfSOInCollection;

        public ItemSO AssociatedItemSO()
        {
            return ItemCollectionManager.Instance.GetItemSObyIndex(indexOfSOInCollection);
        }

        public void OnItemAddedToInventory(Entity entity)
        {
            if (consumable) count++;
            entityOfInventory = entity;
            inventory = entityOfInventory.GetComponent<IInventoryable>();
            OnItemAddedEffects(entity);
            if (entityOfInventory.GetComponent<Minion>())
            {
                foreach (var index in AssociatedItemSO().MinionPassiveCapacitiesIndexes)
                {
                    entityOfInventory.AddPassiveCapacityRPC(index);
                }

                activeCapacities.Clear();
                foreach (var index in AssociatedItemSO().MinionActiveCapacitiesIndexes)
                {
                    activeCapacities.Add(CapacitySOCollectionManager.CreateActiveCapacity(index, entityOfInventory));
                }

                return;
            }
            if (entityOfInventory.GetComponent<Pinata>())
            {
                foreach (var index in AssociatedItemSO().PinataPassiveCapacitiesIndexes)
                {
                    entityOfInventory.AddPassiveCapacityRPC(index);
                }

                activeCapacities.Clear();
                foreach (var index in AssociatedItemSO().PinataActiveCapacitiesIndexes)
                {
                    activeCapacities.Add(CapacitySOCollectionManager.CreateActiveCapacity(index, entityOfInventory));
                }

                return;
            }

            foreach (var index in AssociatedItemSO().passiveCapacitiesIndexes)
            {
                entityOfInventory.AddPassiveCapacityRPC(index);
            }

            activeCapacities.Clear();
            foreach (var index in AssociatedItemSO().activeCapacitiesIndexes)
            {
                activeCapacities.Add(CapacitySOCollectionManager.CreateActiveCapacity(index, entityOfInventory));
            }
        }

        protected abstract void OnItemAddedEffects(Entity entity);

        public void OnItemAddedToInventoryFeedback(Entity entity)
        {
            entityOfInventory = entity;
            inventory = entityOfInventory.GetComponent<IInventoryable>();
            OnItemAddedEffectsFeedback(entity);
        }

        protected abstract void OnItemAddedEffectsFeedback(Entity entity);

        public void OnItemRemovedFromInventory(Entity entity)
        {
            OnItemRemovedEffects(entity);
            if (!entity.GetComponent<Minion>())
                foreach (var index in AssociatedItemSO().passiveCapacitiesIndexes)
                {
                    entityOfInventory.RemovePassiveCapacityByIndexRPC(index);
                }
            else
            {
                foreach (var index in AssociatedItemSO().MinionPassiveCapacitiesIndexes)
                {
                    entityOfInventory.RemovePassiveCapacityByIndexRPC(index);
                }
            }
        }

        protected abstract void OnItemRemovedEffects(Entity entity);

        public void OnItemRemovedFromInventoryFeedback(Entity entity)
        {
            OnItemRemovedEffectsFeedback(entity);
        }

        protected abstract void OnItemRemovedEffectsFeedback(Entity entity);

        public void OnItemActivated(int target, Vector3 position)
        {
            if (consumable) count--;
            if (isMaster) OnItemActivatedEffects(target, position);

            OnItemActivatedFeedbackEffects(target, position);

            if (isMaster && count <= 0) inventory.RemoveItemRPC(this);
        }

        public abstract void OnItemActivatedEffects(int targetIndex, Vector3 position);
        public abstract void OnItemActivatedFeedbackEffects(int targetIndex, Vector3 position);
    }
}