using Photon.Pun;

namespace Entities.Champion
{
    public partial class Champion : IActiveLifeable
    {
        public float maxHp;
        public float currentHp;

        public float GetMaxHp()
        {
            return maxHp;
        }

        public float GetCurrentHp()
        {
            return currentHp;
        }

        public float GetCurrentHpPercent()
        {
            return currentHp / maxHp * 100f;
        }

        public void RequestSetMaxHp(float value)
        {
            if(isMaster)
            {
                SetMaxHpRPC(value);
                return;
            }
            photonView.RPC("SetMaxHpRPC", RpcTarget.MasterClient, value);
        }

        [PunRPC]
        public void SetMaxHpRPC(float value)
        {
            maxHp = value;
            currentHp = value;
            OnSetMaxHp?.Invoke(value);
            if (isOffline)
            {
                OnSetMaxHpFeedback?.Invoke(value);
                return;
            }
            photonView.RPC("SyncSetMaxHpRPC", RpcTarget.All, maxHp);
        }
        
        [PunRPC]
        public void SyncSetMaxHpRPC(float value)
        {
            maxHp = value;
            currentHp = value;
            OnSetMaxHpFeedback?.Invoke(value);
        }

        public event GlobalDelegates.FloatDelegate OnSetMaxHp;
        public event GlobalDelegates.FloatDelegate OnSetMaxHpFeedback;

        public void RequestIncreaseMaxHp(float amount)
        {
            if(isMaster)
            {
                IncreaseMaxHpRPC(amount);
                return;
            }
            photonView.RPC("IncreaseMaxHpRPC", RpcTarget.MasterClient, amount);
        }
        
        [PunRPC]
        public void IncreaseMaxHpRPC(float amount)
        {
            maxHp += amount;
            currentHp += amount;
            if (currentHp > maxHp) currentHp = maxHp;
            OnIncreaseMaxHp?.Invoke(amount);
            if (isOffline) return;
            photonView.RPC("SyncIncreaseMaxHpRPC", RpcTarget.All, maxHp);
        }

        [PunRPC]
        public void SyncIncreaseMaxHpRPC(float newMaxHp)
        {
            var gainedHp = newMaxHp - maxHp;
            maxHp = newMaxHp;
            currentHp += gainedHp;
            if (currentHp > maxHp) currentHp = maxHp;
            OnIncreaseMaxHpFeedback?.Invoke(gainedHp);
        }

        public event GlobalDelegates.FloatDelegate OnIncreaseMaxHp;
        public event GlobalDelegates.FloatDelegate OnIncreaseMaxHpFeedback;

        public void RequestDecreaseMaxHp(float amount)
        {
            if(isMaster)
            {
                DecreaseMaxHpRPC(amount);
                return;
            }
            photonView.RPC("DecreaseMaxHpRPC", RpcTarget.MasterClient, amount);
        }
        
        [PunRPC]
        public void DecreaseMaxHpRPC(float amount)
        {
            maxHp -= amount;
            if(currentHp>maxHp) currentHp = maxHp;
            OnDecreaseMaxHp?.Invoke(amount);
            if (isOffline)
            {
                OnDecreaseMaxHpFeedback?.Invoke(amount);
                return;
            }
            photonView.RPC("SyncDecreaseMaxHpRPC", RpcTarget.All, maxHp);
        }

        [PunRPC]
        public void SyncDecreaseMaxHpRPC(float newMaxHp)
        {
            var lostHp = maxHp - newMaxHp;
            maxHp = newMaxHp;
            currentHp -= lostHp;
            if (currentHp > maxHp) currentHp = maxHp;
            OnDecreaseMaxHpFeedback?.Invoke(lostHp);
        }

        public event GlobalDelegates.FloatDelegate OnDecreaseMaxHp;
        public event GlobalDelegates.FloatDelegate OnDecreaseMaxHpFeedback;

        public void RequestSetCurrentHp(float value)
        {
            if(isMaster)
            {
                SetCurrentHpRPC(value);
                return;
            }
            photonView.RPC("SetCurrentHpRPC", RpcTarget.MasterClient, value);
        }
        
        [PunRPC]
        public void SetCurrentHpRPC(float value)
        {
            currentHp = value;
            OnSetCurrentHp?.Invoke(value);
            if (isOffline)
            {
                OnSetCurrentHpFeedback?.Invoke(value);
                return;
            }
            photonView.RPC("SyncSetCurrentHpRPC", RpcTarget.All, value);
        }

        [PunRPC]
        public void SyncSetCurrentHpRPC(float value)
        {
            currentHp = value;
            OnSetCurrentHpFeedback?.Invoke(value);
        }

        public event GlobalDelegates.FloatDelegate OnSetCurrentHp;
        public event GlobalDelegates.FloatDelegate OnSetCurrentHpFeedback;

        public void RequestSetCurrentHpPercent(float value)
        {
            if(isMaster)
            {
                SetCurrentHpPercentRPC(value);
                return;
            }
            photonView.RPC("SetCurrentHpPercentRPC", RpcTarget.MasterClient, value);
        }

        [PunRPC]
        public void SetCurrentHpPercentRPC(float value)
        {
            currentHp = maxHp * value * 0.01f;
            if (currentHp > maxHp) currentHp = maxHp;
            OnSetCurrentHpPercent?.Invoke(value);
            if (isOffline)
            {
                OnSetCurrentHpPercentFeedback?.Invoke(value);
                return;
            }
            photonView.RPC("SyncSetCurrentHpPercentRPC", RpcTarget.All, currentHp);
        }
        
        [PunRPC]
        public void SyncSetCurrentHpPercentRPC(float value)
        {
            currentHp = value;
            OnSetCurrentHpPercentFeedback?.Invoke(value);
        }

        public event GlobalDelegates.FloatDelegate OnSetCurrentHpPercent;
        public event GlobalDelegates.FloatDelegate OnSetCurrentHpPercentFeedback;

        public void RequestIncreaseCurrentHp(float amount)
        {
            if(isMaster)
            {
                IncreaseCurrentHpRPC(amount);
                return;
            }
            photonView.RPC("IncreaseCurrentHpRPC",RpcTarget.MasterClient,amount);
        }
        
        [PunRPC]
        public void IncreaseCurrentHpRPC(float amount)
        {
            currentHp += amount;
            if (currentHp > maxHp) currentHp = maxHp;
            OnIncreaseCurrentHp?.Invoke(amount);
            if (isOffline)
            {
                OnIncreaseCurrentHpFeedback?.Invoke(amount);
                return;
            }
            photonView.RPC("SyncIncreaseCurrentHpRPC",RpcTarget.All,currentHp);
        }

        [PunRPC]
        public void SyncIncreaseCurrentHpRPC(float amount)
        {
            currentHp = amount;
            OnIncreaseCurrentHpFeedback?.Invoke(amount);
        }

        public event GlobalDelegates.FloatDelegate OnIncreaseCurrentHp;
        public event GlobalDelegates.FloatDelegate OnIncreaseCurrentHpFeedback;

        public void RequestDecreaseCurrentHp(float amount)
        {
            if(isMaster)
            {
                DecreaseCurrentHpRPC(amount);
                return;
            }
            photonView.RPC("DecreaseCurrentHpRPC",RpcTarget.MasterClient,amount);
        }
        
        [PunRPC]
        public void DecreaseCurrentHpRPC(float amount)
        {
            currentHp -= amount;
            OnDecreaseCurrentHp?.Invoke(amount);
            if (isOffline)
            {
                OnDecreaseCurrentHpFeedback?.Invoke(amount);
                return;
            }
            photonView.RPC("SyncDecreaseCurrentHpRPC",RpcTarget.All,currentHp);
        }

        [PunRPC]
        public void SyncDecreaseCurrentHpRPC(float amount)
        {
            currentHp = amount;
            if (currentHp <= 0)
            {
                currentHp = 0;
                RequestDie();
            }
            OnDecreaseCurrentHpFeedback?.Invoke(amount);
        }
        
        public event GlobalDelegates.FloatDelegate OnDecreaseCurrentHp;
        public event GlobalDelegates.FloatDelegate OnDecreaseCurrentHpFeedback;
    }
}