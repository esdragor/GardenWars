using System;
using System.Collections.Generic;
using System.Linq;
using Entities.Capacities;
using Entities.Inventory;
using Photon.Pun;
using UnityEngine;

namespace Entities.Champion
{
    public partial class Champion : IInventoryable
    {
        [SerializeReference] public List<Item> items = new List<Item>();
        private readonly List<Item> heldItems = new List<Item>();

        public int selectedItemIndex = 0;

        public Item[] GetItems()
        {
            return items.ToArray();
        }

        public Item GetItem(int index)
        {
            if (index < 0 || index >= items.Count) return null;
            return items[index];
        }

        public Item GetItemOfSo(int soIndex)
        {
            return items.FirstOrDefault(item => item.indexOfSOInCollection == soIndex);
        }

        public bool CanAddItem(byte soIndex)
        {
            var itemSo = ItemCollectionManager.Instance.GetItemSObyIndex(soIndex);
            if (items.Count < 3) return true;
            var contains = false;
            foreach (var item in items.Where(item => item.indexOfSOInCollection == soIndex))
            {
                contains = true;
            }
            return (itemSo.consumable && contains);
        }
        

        public void RequestAddItem(byte index)
        {
            if (isMaster)
            {
                AddItemRPC(index);
                return;
            }
            photonView.RPC("AddItemRPC",RpcTarget.MasterClient,index);
        }

        [PunRPC]
        public void AddItemRPC(byte index)
        {
            var itemSo = ItemCollectionManager.Instance.GetItemSObyIndex(index);
            if (itemSo.consumable)
            {
                var contains = false;
                foreach (var item in items.Where(item => item.indexOfSOInCollection == index))
                {
                    contains = true;
                }
                if(!contains && items.Count>=3) return;
                
                if (isOffline)
                {
                    SyncAddItemRPC(index);
                    return;
                }
                photonView.RPC("SyncAddItemRPC",RpcTarget.All, index);
                return;
            }
            
            if (isOffline)
            {
                SyncAddItemRPC(index);
                return;
            }
            if(items.Count>=3) return;
            photonView.RPC("SyncAddItemRPC",RpcTarget.All, index);
        }

        [PunRPC]
        public void SyncAddItemRPC(byte index)
        {
            var item = ItemCollectionManager.Instance.CreateItem(index, this);
            if(item == null) return;
            if (!items.Contains(item)) items.Add(item);
            if (isMaster)
            {
                item.OnItemAddedToInventory(this);
                OnAddItem?.Invoke(index);
            }

            item.OnItemAddedToInventoryFeedback(this);
            OnAddItemFeedback?.Invoke(index);
        }

        public event GlobalDelegates.ByteDelegate OnAddItem;
        public event GlobalDelegates.ByteDelegate OnAddItemFeedback;
        
        /// <param name="index">index of Item in this entity's inventory (not in item Collection)</param>
        public void RequestRemoveItem(byte index)
        {
            if (isMaster)
            {
                RemoveItemRPC(index);
                return;
            }
            photonView.RPC("RemoveItemRPC",RpcTarget.MasterClient,index);
        }

        /// <param name="item">Item to remove from this entity's inventory</param>
        public void RequestRemoveItem(Item item)
        {
            if(!items.Contains(item)) return;
            RequestRemoveItem((byte)items.IndexOf(item));
        }

        [PunRPC]
        public void RemoveItemRPC(byte index)
        {
            if (isOffline)
            {
                SyncRemoveItemRPC(index);
                return;
            }
            photonView.RPC("SyncRemoveItemRPC",RpcTarget.All,index);
        }

        public void RemoveItemRPC(Item item)
        {
            if(!items.Contains(item)) return;
            var index = items.IndexOf(item);
            RemoveItemRPC((byte)index);
        }

        [PunRPC]
        public void SyncRemoveItemRPC(byte index)
        {
            if(index >= items.Count) return;
            var item = items[index];
            items.Remove(item);
            if(heldItems.Contains(item))heldItems.Remove(item);
            if (PhotonNetwork.IsMasterClient)
            {
                item.OnItemRemovedFromInventory(this);
                OnRemoveItem?.Invoke(index);
            }
            item.OnItemRemovedFromInventoryFeedback(this);
            OnRemoveItemFeedback?.Invoke(index);
        }
        public event GlobalDelegates.ByteDelegate OnRemoveItem;
        public event GlobalDelegates.ByteDelegate OnRemoveItemFeedback;
        
        public void RequestPressItem(byte itemIndexInInventory,int selectedEntities,Vector3 positions)
        {
            selectedItemIndex = itemIndexInInventory;
            if (!isFighter) return;
            if (itemIndexInInventory >= items.Count) return;
            if (isMaster)
            {
                PressItemRPC(itemIndexInInventory, selectedEntities, positions);
                return;
            }

            photonView.RPC("PressItemRPC", RpcTarget.MasterClient, itemIndexInInventory, selectedEntities,
                positions);
        }

        [PunRPC]
        public void PressItemRPC(byte itemIndexInInventory,int selectedEntities,Vector3 positions)
        {
            selectedItemIndex = itemIndexInInventory;
            if (!isFighter) return;
            if(itemIndexInInventory >= items.Count) return;
            var item = items[itemIndexInInventory];
            if(item == null) return;
            if (isOffline)
            {
                SyncPressItemRPC(itemIndexInInventory, selectedEntities, positions);
                return;
            }
            photonView.RPC("SyncPressItemRPC",RpcTarget.All,itemIndexInInventory,selectedEntities,positions);
        }

        [PunRPC]
        public void SyncPressItemRPC(byte itemIndexInInventory,int selectedEntities,Vector3 positions)
        {
            selectedItemIndex = itemIndexInInventory;
            if (!isFighter) return;
            targetedEntities = selectedEntities;
            targetedPositions = positions;
            if(itemIndexInInventory >= items.Count) return;
            var item = items[itemIndexInInventory];
            if(items[itemIndexInInventory] == null) return;
            foreach (var activeCapacity in item.activeCapacities)
            {
                activeCapacity.OnPress(targetedEntities,targetedPositions);
            }
            heldItems.Add(item);
        }
        
        private void CastHeldItems()
        {
            if (!isFighter) return;
            foreach (var capacity in heldItems.SelectMany(item => item.activeCapacities))
            {
                capacity.OnHold(targetedEntities,targetedPositions);
            }
        }
        
        public void RequestReleaseItem(byte itemIndexInInventory,int selectedEntities,Vector3 positions)
        {
            if (isMaster)
            {
                ReleaseItemRPC(itemIndexInInventory,selectedEntities,positions);
                return;
            }
            photonView.RPC("ReleaseItemRPC",RpcTarget.MasterClient,itemIndexInInventory,selectedEntities,positions);
        }

        [PunRPC]
        public void ReleaseItemRPC(byte itemIndexInInventory,int selectedEntities,Vector3 positions)
        {
            if (isFighter)
            {
                if (itemIndexInInventory >= items.Count) return;
                var item = items[itemIndexInInventory];
                if (item == null) return;
                items[itemIndexInInventory].OnItemActivated(selectedEntities, positions);
                OnActivateItem?.Invoke(itemIndexInInventory, selectedEntities, positions);
            }
            else
            {
                OnActivateItem?.Invoke(itemIndexInInventory, selectedEntities, positions);
            }
            if (isOffline)
            {
                SyncReleaseItemRPC(itemIndexInInventory, selectedEntities, positions);
                return;
            }
            photonView.RPC("SyncReleaseItemRPC", RpcTarget.All, itemIndexInInventory, selectedEntities, positions);
        }

        [PunRPC]
        public void SyncReleaseItemRPC(byte itemIndexInInventory,int selectedEntities,Vector3 positions)
        {
            if (!isFighter)
            {
                OnActivateItemFeedback?.Invoke(itemIndexInInventory,selectedEntities,positions);
                return;
            }
            if(itemIndexInInventory >= items.Count) return;
            var item = items[itemIndexInInventory];
            if(items[itemIndexInInventory] == null) return;
            heldItems.Remove(item);
            foreach (var activeCapacity in item.activeCapacities)
            {
                activeCapacity.OnRelease(selectedEntities,positions);
            }
            items[itemIndexInInventory].OnItemActivatedFeedback(selectedEntities,positions);
            OnActivateItemFeedback?.Invoke(itemIndexInInventory,selectedEntities,positions);
        }
        
        public event GlobalDelegates.ByteIntArrayVector3ArrayDelegate OnActivateItem;
        public event GlobalDelegates.ByteIntArrayVector3ArrayDelegate OnActivateItemFeedback;
        
        public Item PopSelectedItem()
        {
            var item = GetItem(selectedItemIndex);
            if(item != null) RemoveItemRPC(item);
            return item;
        }
    }
}