using System.Collections.Generic;
using System.Linq;
using Entities.Inventory;
using Photon.Pun;
using UnityEngine;

namespace Entities
{
    public partial class Entity : IInventoryable
    {
        [Header("Inventory")]
        [SerializeField] protected int maxItems; 
        [SerializeReference] public List<Item> items = new List<Item>();
        protected readonly List<Item> heldItems = new List<Item>();
        
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
            if (items.Count < maxItems) return true;
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
            if(!CanAddItem(index)) return;
            
            if (isOffline)
            {
                SyncAddItemRPC(index);
                return;
            }
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
            if(items.Count == 0) return;
            var item = items[index];
            items.Remove(item);
            if(heldItems.Contains(item))heldItems.Remove(item);
            if (isMaster)
            {
                item.OnItemRemovedFromInventory(this);
                OnRemoveItem?.Invoke(index);
            }
            item.OnItemRemovedFromInventoryFeedback(this);
            OnRemoveItemFeedback?.Invoke(index);
        }
        public event GlobalDelegates.ByteDelegate OnRemoveItem;
        public event GlobalDelegates.ByteDelegate OnRemoveItemFeedback;
    }
}