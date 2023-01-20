using Photon.Pun;
using UnityEngine;

namespace Entities.Champion
{
    public partial class Champion : ICandyable
    {
        [Header("Candy")]
        [SerializeField] private int maxCandy;
        public int currentCandy { get; private set; }
        
        public int GetMaxCandy()
        {
            return maxCandy;
        }

        public int GetCurrentCandy()
        {
            return currentCandy;
        }

        public float GetCurrentCandyPercent()
        {
            return (float)currentCandy / maxCandy * 100f;
        }

        public void RequestSetMaxCandy(int value)
        {
            if (isMaster)
            {
                SetMaxCandyRPC(value);
                return;
            }

            photonView.RPC("SetMaxCandyRPC", RpcTarget.MasterClient, value);
        }

        [PunRPC]
        public void SetMaxCandyRPC(int value)
        {
            maxCandy = value;
            currentCandy = value;
            OnSetMaxCandy?.Invoke(value);
            if (isOffline)
            {
                OnSetMaxCandyFeedback?.Invoke(value);
                return;
            }

            photonView.RPC("SyncSetMaxCandyRPC", RpcTarget.All, maxCandy);
        }

        [PunRPC]
        public void SyncSetMaxCandyRPC(int value)
        {
            maxCandy = value;
            currentCandy = value;
            OnSetMaxCandyFeedback?.Invoke(value);
        }

        public event GlobalDelegates.IntDelegate OnSetMaxCandy;
        public event GlobalDelegates.IntDelegate OnSetMaxCandyFeedback;

        public void RequestIncreaseMaxCandy(int amount)
        {
            if (isMaster)
            {
                IncreaseMaxCandyRPC(amount);
                return;
            }

            photonView.RPC("IncreaseMaxCandyRPC", RpcTarget.MasterClient, amount);
        }

        [PunRPC]
        public void IncreaseMaxCandyRPC(int amount)
        {
            maxCandy += amount;
            currentCandy += amount;
            if (currentCandy > maxCandy) currentCandy = maxCandy;
            OnIncreaseMaxCandy?.Invoke(amount);
            if (isOffline) return;
            photonView.RPC("SyncIncreaseMaxCandyRPC", RpcTarget.All, maxCandy);
        }

        [PunRPC]
        public void SyncIncreaseMaxCandyRPC(int newMaxCandy)
        {
            var gainedCandy = newMaxCandy - maxCandy;
            maxCandy = newMaxCandy;
            currentCandy += gainedCandy;
            if (currentCandy > maxCandy) currentCandy = maxCandy;
            OnIncreaseMaxCandyFeedback?.Invoke(gainedCandy);
        }

        public event GlobalDelegates.IntDelegate OnIncreaseMaxCandy;
        public event GlobalDelegates.IntDelegate OnIncreaseMaxCandyFeedback;

        public void RequestDecreaseMaxCandy(int amount)
        {
            if (isMaster)
            {
                DecreaseMaxCandyRPC(amount);
                return;
            }

            photonView.RPC("DecreaseMaxCandyRPC", RpcTarget.MasterClient, amount);
        }

        [PunRPC]
        public void DecreaseMaxCandyRPC(int amount)
        {
            maxCandy -= amount;
            if (currentCandy > maxCandy) currentCandy = maxCandy;
            OnDecreaseMaxCandy?.Invoke(amount);
            if (isOffline)
            {
                OnDecreaseMaxCandyFeedback?.Invoke(amount);
                return;
            }

            photonView.RPC("SyncDecreaseMaxCandyRPC", RpcTarget.All, maxCandy);
        }

        [PunRPC]
        public void SyncDecreaseMaxCandyRPC(int newMaxCandy)
        {
            var lostCandy = maxCandy - newMaxCandy;
            maxCandy = newMaxCandy;
            currentCandy -= lostCandy;
            if (currentCandy > maxCandy) currentCandy = maxCandy;
            OnDecreaseMaxCandyFeedback?.Invoke(lostCandy);
        }

        public event GlobalDelegates.IntDelegate OnDecreaseMaxCandy;
        public event GlobalDelegates.IntDelegate OnDecreaseMaxCandyFeedback;

        public void RequestSetCurrentCandy(int value)
        {
            if (isMaster)
            {
                SetCurrentCandyRPC(value);
                return;
            }

            photonView.RPC("SetCurrentCandyRPC", RpcTarget.MasterClient, value);
        }

        [PunRPC]
        public void SetCurrentCandyRPC(int value)
        {
            currentCandy = value;
            OnSetCurrentCandy?.Invoke(value);
            if (isOffline)
            {
                OnSetCurrentCandyFeedback?.Invoke(value);
                return;
            }

            photonView.RPC("SyncSetCurrentCandyRPC", RpcTarget.All, value);
        }

        [PunRPC]
        public void SyncSetCurrentCandyRPC(int value)
        {
            currentCandy = value;
            OnSetCurrentCandyFeedback?.Invoke(value);
        }

        public event GlobalDelegates.IntDelegate OnSetCurrentCandy;
        public event GlobalDelegates.IntDelegate OnSetCurrentCandyFeedback;

        public void RequestSetCurrentCandyPercent(int value)
        {
            if (isMaster)
            {
                SetCurrentCandyPercentRPC(value);
                return;
            }

            photonView.RPC("SetCurrentCandyPercentRPC", RpcTarget.MasterClient, value);
        }

        [PunRPC]
        public void SetCurrentCandyPercentRPC(int value)
        {
            currentCandy = Mathf.RoundToInt(maxCandy * value * 0.01f);
            if (currentCandy > maxCandy) currentCandy = maxCandy;
            OnSetCurrentCandyPercent?.Invoke(value);
            if (isOffline)
            {
                OnSetCurrentCandyPercentFeedback?.Invoke(value);
                return;
            }

            photonView.RPC("SyncSetCurrentCandyPercentRPC", RpcTarget.All, currentCandy);
        }

        [PunRPC]
        public void SyncSetCurrentCandyPercentRPC(int value)
        {
            currentCandy = value;
            OnSetCurrentCandyPercentFeedback?.Invoke(value);
        }

        public event GlobalDelegates.IntDelegate OnSetCurrentCandyPercent;
        public event GlobalDelegates.IntDelegate OnSetCurrentCandyPercentFeedback;

        public void RequestIncreaseCurrentCandy(int amount)
        {
            if (isMaster)
            {
                IncreaseCurrentCandyRPC(amount);
                return;
            }

            photonView.RPC("IncreaseCurrentCandyRPC", RpcTarget.MasterClient, amount);
        }

        [PunRPC]
        public void IncreaseCurrentCandyRPC(int amount)
        {
            currentCandy += amount;
            
            if (currentCandy > maxCandy) currentCandy = maxCandy;
            OnIncreaseCurrentCandy?.Invoke(amount);
            if (isOffline)
            {
                OnIncreaseCurrentCandyFeedback?.Invoke(amount);
                return;
            }

            photonView.RPC("SyncIncreaseCurrentCandyRPC", RpcTarget.All, currentCandy,upgradeCount);
        }

        [PunRPC]
        public void SyncIncreaseCurrentCandyRPC(int amount,int upgrades)
        {
            currentCandy = amount;
            upgradeCount = upgrades;
            OnIncreaseCurrentCandyFeedback?.Invoke(amount);
        }

        public event GlobalDelegates.IntDelegate OnIncreaseCurrentCandy;
        public event GlobalDelegates.IntDelegate OnIncreaseCurrentCandyFeedback;

        public void RequestDecreaseCurrentCandy(int amount)
        {
            if (isMaster)
            {
                DecreaseCurrentCandyRPC(amount);
                return;
            }

            photonView.RPC("DecreaseCurrentCandyRPC", RpcTarget.MasterClient, amount);
        }

        [PunRPC]
        public void DecreaseCurrentCandyRPC(int amount)
        {
            currentCandy -= amount;
            OnDecreaseCurrentCandy?.Invoke(amount);
            if (isOffline)
            {
                OnDecreaseCurrentCandyFeedback?.Invoke(amount);
                return;
            }
            photonView.RPC("SyncDecreaseCurrentCandyRPC", RpcTarget.All, currentCandy);
        }

        [PunRPC]
        public void SyncDecreaseCurrentCandyRPC(int amount)
        {
            currentCandy = amount;
            if (currentCandy <= 0) currentCandy = 0;

            OnDecreaseCurrentCandyFeedback?.Invoke(amount);
        }

        public event GlobalDelegates.IntDelegate OnDecreaseCurrentCandy;
        public event GlobalDelegates.IntDelegate OnDecreaseCurrentCandyFeedback;


        public void RequestChannelPinata(Pinata pinata)
        {
            if (isMaster)
            {
                ChannelPinataRPC(pinata.entityIndex);
                return;
            }
                
            photonView.RPC("ChannelPinataRPC", RpcTarget.MasterClient, pinata.entityIndex);
        }

        [PunRPC]
        public void ChannelPinataRPC(int pinataIndex)
        {
            var entity = EntityCollectionManager.GetEntityByIndex(pinataIndex);
            if (entity is Pinata pinata)
            {
                pinata.StartChanneling(this);
            }
        }
    }
}