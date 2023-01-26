using System;
using Photon.Pun;
using UnityEngine;

namespace Entities.Champion
{
    public partial class Champion : IActiveLifeable
    {
        public float maxHp;
        public float currentHp;

        public float maxDef = 40;
        public float baseDef = 0;
        public float def = 0;
        public float actualDef => baseDef + def < maxDef ? baseDef + def : maxDef;

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

        public void RequestIncreaseMaxHp(float amount, int killerId)
        {
            if(isMaster)
            {
                IncreaseMaxHpRPC(amount, killerId);
                return;
            }
            photonView.RPC("IncreaseMaxHpRPC", RpcTarget.MasterClient, amount, killerId);
        }
        
        [PunRPC]
        public void IncreaseMaxHpRPC(float amount, int killerId)
        {
            maxHp += amount;
            currentHp += amount;
            if (currentHp > maxHp) currentHp = maxHp;
            OnIncreaseMaxHp?.Invoke(amount);
            if (isOffline) return;
            photonView.RPC("SyncIncreaseMaxHpRPC", RpcTarget.All, maxHp, killerId);
        }

        [PunRPC]
        public void SyncIncreaseMaxHpRPC(float newMaxHp, int killerId)
        {
            var gainedHp = newMaxHp - maxHp;
            maxHp = newMaxHp;
            currentHp += gainedHp;
            if (currentHp > maxHp) currentHp = maxHp;
            OnIncreaseMaxHpFeedback?.Invoke(gainedHp);
        }

        public event GlobalDelegates.FloatDelegate OnIncreaseMaxHp;
        public event GlobalDelegates.FloatDelegate OnIncreaseMaxHpFeedback;

        public void RequestDecreaseMaxHp(float amount, int killerId)
        {
            if(isMaster)
            {
                DecreaseMaxHpRPC(amount, killerId);
                return;
            }
            photonView.RPC("DecreaseMaxHpRPC", RpcTarget.MasterClient, amount, killerId);
        }
        
        [PunRPC]
        public void DecreaseMaxHpRPC(float amount, int killerId)
        {
            maxHp -= amount;
            if(currentHp>maxHp) currentHp = maxHp;
            OnDecreaseMaxHp?.Invoke(amount);
            if (isOffline)
            {
                OnDecreaseMaxHpFeedback?.Invoke(amount);
                return;
            }
            photonView.RPC("SyncDecreaseMaxHpRPC", RpcTarget.All, maxHp, killerId);
        }

        [PunRPC]
        public void SyncDecreaseMaxHpRPC(float newMaxHp, int killerId)
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

        public void RequestIncreaseCurrentHp(float amount, int killerId)
        {
            if(isMaster)
            {
                IncreaseCurrentHpRPC(amount, killerId);
                return;
            }
            photonView.RPC("IncreaseCurrentHpRPC",RpcTarget.MasterClient,amount, killerId);
        }
        
        [PunRPC]
        public void IncreaseCurrentHpRPC(float amount, int killerId)
        {
            currentHp += amount;
            if (currentHp > maxHp) currentHp = maxHp;
            OnIncreaseCurrentHp?.Invoke(amount,killerId);
            if (isOffline)
            {
                OnIncreaseCurrentHpFeedback?.Invoke(amount,killerId);
                return;
            }
            photonView.RPC("SyncIncreaseCurrentHpRPC",RpcTarget.All,currentHp, killerId);
        }

        [PunRPC]
        public void SyncIncreaseCurrentHpRPC(float amount, int killerId)
        {
            currentHp = amount;
            OnIncreaseCurrentHpFeedback?.Invoke(amount,killerId);
        }

        public event Action<float,int> OnIncreaseCurrentHp;
        public event Action<float,int> OnIncreaseCurrentHpFeedback;

        public void RequestDecreaseCurrentHp(float amount, int killerId)
        {
            if(isMaster)
            {
                DecreaseCurrentHpRPC(amount, killerId);
                return;
            }
            photonView.RPC("DecreaseCurrentHpRPC",RpcTarget.MasterClient,amount, killerId);
        }
        
        [PunRPC]
        public void DecreaseCurrentHpRPC(float amount, int killerId)
        {
            amount *= (1 - actualDef / 100f);

            OnDecreaseCurrentHp?.Invoke(amount,killerId);
            
            if (isOffline)
            {
                OnDecreaseCurrentHpFeedback?.Invoke(amount,killerId);
                return;
            }

            photonView.RPC("SyncDecreaseCurrentHpRPC", RpcTarget.All, currentHp - amount, killerId);
        }

        [PunRPC]
        public void SyncDecreaseCurrentHpRPC(float amount, int killerId)
        {
            var current = currentHp;
            currentHp = amount;
            var lost = current - currentHp;
            if (currentHp <= 0)
            {
                currentHp = 0;
                if(isPlayerChampion) RequestDie(killerId);
            }
            OnDecreaseCurrentHpFeedback?.Invoke(lost,killerId);
        }
        
        public event Action<float,int> OnDecreaseCurrentHp;
        public event Action<float,int> OnDecreaseCurrentHpFeedback;
        
        public void RequestIncreaseDef(float amount)
        {
            if(isMaster)
            {
                IncreaseDefRPC(amount);
                return;
            }
            photonView.RPC("IncreaseDefRPC", RpcTarget.MasterClient, amount);
        }

        [PunRPC]
        public void IncreaseDefRPC(float amount)
        {
            def += amount;
            if (isOffline)
            {
                SyncIncreaseDefRPC(def);
                return;
            }
            photonView.RPC("SyncIncreaseDefRPC", RpcTarget.All, def);
        }

        [PunRPC]
        public void SyncIncreaseDefRPC(float newDef)
        {
            def = newDef;
        }

        public void RequestDecreaseDefHp(float amount)
        {
            if(isMaster)
            {
                DecreaseDefRPC(amount);
                return;
            }
            photonView.RPC("DecreaseDefRPC", RpcTarget.MasterClient, amount);
        }
        
        [PunRPC]
        public void DecreaseDefRPC(float amount)
        {
            def -= amount;
            if (isOffline)
            {
                SyncDecreaseDefRPC(def);
                return;
            }
            photonView.RPC("SyncDecreaseDefRPC", RpcTarget.All, def);
        }

        [PunRPC]
        public void SyncDecreaseDefRPC(float newDef)
        {
            def = newDef;
        }
    }
}