using System;
using Entities.Capacities;
using UnityEngine;
using Photon.Pun;

namespace Entities.Champion
{
    public partial class Champion : IAttackable
    {
        [Header("Attack")]
        public byte attackAbilityIndex;
        public bool canAttack;
        public float attackDamage;
        public double baseAttackSpeed;
        public double bonusAttackSpeed;
        public double attackSpeed => baseAttackSpeed - bonusAttackSpeed;
        public float attackRange;

        public bool CanAttack()
        {
            return canAttack;
        }

        public void RequestSetCanAttack(bool value)
        {
            if (isMaster)
            {
                SetCanAttackRPC(value);
                return;
            }
            photonView.RPC("SetCanAttackRPC", RpcTarget.MasterClient, value);
        }
        
        [PunRPC]
        public void SetCanAttackRPC(bool value)
        {
            canAttack = value;
            OnSetCanAttack?.Invoke(value);
            if (isOffline)
            {
                SyncSetCanAttackRPC(value);
                return;
            }
            photonView.RPC("SyncSetCanAttackRPC", RpcTarget.All, value);
        }

        [PunRPC]
        public void SyncSetCanAttackRPC(bool value)
        {
            canAttack = value;
            OnSetCanAttackFeedback?.Invoke(value);
        }

        public event GlobalDelegates.BoolDelegate OnSetCanAttack;
        public event GlobalDelegates.BoolDelegate OnSetCanAttackFeedback;
        public float GetAttackDamage() => attackDamage;

        public void RequestSetAttackDamage(float value)
        {
            if (isMaster)
            {
                SetAttackDamageRPC(value);
                return;
            }
            photonView.RPC("SetAttackDamageRPC",RpcTarget.MasterClient,value);
        }

        public void SetAttackDamageRPC(float value)
        {
            attackDamage = value;
            OnSetAttackDamage?.Invoke(value);
            if (isOffline)
            {
                SyncSetAttackDamageRPC(value);
                return;
            }
            photonView.RPC("SyncSetAttackDamageRPC",RpcTarget.All,attackDamage);
        }
        
        public void SyncSetAttackDamageRPC(float value)
        {
            attackDamage = value;
            OnSetAttackDamageFeedback?.Invoke(value);
        }
        
        public event GlobalDelegates.FloatDelegate OnSetAttackDamage;
        public event GlobalDelegates.FloatDelegate OnSetAttackDamageFeedback;
        
        public void RequestAttack(byte attackIndex, int targetedEntities, Vector3 targetedPositions)
        {
            if(isMaster)
            {
                AttackRPC(attackIndex,targetedEntities,targetedPositions);
                return;
            }
            photonView.RPC("AttackRPC",RpcTarget.MasterClient,attackIndex,targetedEntities,targetedPositions);
        }

        [PunRPC]
        public void AttackRPC(byte attackIndex, int newTargetedEntities, Vector3 newTargetedPositions)
        {
            if(!canAttack) return;
            
            if (isOffline)
            {
                SyncAttackRPC(attackIndex, newTargetedEntities, newTargetedPositions);
                return;
            }
            photonView.RPC("SyncAttackRPC",RpcTarget.All,attackIndex,newTargetedEntities,newTargetedPositions);
        }

        [PunRPC]
        public void SyncAttackRPC(byte attackIndex, int newTargetedEntities, Vector3 newTargetedPositions)
        {
            targetedEntities = newTargetedEntities;
            targetedPositions = newTargetedPositions;
            if (capacityDict.ContainsKey(attackIndex))
            {
                capacityDict[attackIndex].isCasting = false;
            }
            else
            {
                Debug.Log($"Creating new capacity of {attackIndex}");
                var newCapacity = new CastingAbility
                {
                    isCasting = false,
                    capacity = CapacitySOCollectionManager.CreateActiveCapacity(attackIndex,this)
                };
                capacityDict.Add(attackIndex,newCapacity);
            }
            
            capacityDict[attackIndex].capacity.OnRelease(targetedEntities,targetedPositions);
            if(isMaster) OnAttack?.Invoke(attackIndex,targetedEntities,targetedPositions);
            OnAttackFeedback?.Invoke(attackIndex,targetedEntities,targetedPositions);
        }

        public event GlobalDelegates.ByteIntArrayVector3ArrayDelegate OnAttack;
        public event GlobalDelegates.ByteIntArrayVector3ArrayDelegate OnAttackFeedback;
        
        public void RequestChangeAttackSpeed(float attackSpeedMod)
        {
            if(isMaster)
            {
                ChangeAttackSpeedRPC(attackSpeedMod);
                return;
            }
            photonView.RPC("ChangeAttackSpeedRPC",RpcTarget.MasterClient,attackSpeedMod);
        }

        [PunRPC]
        public void ChangeAttackSpeedRPC(float attackSpeedMod)
        {
            bonusAttackSpeed += attackSpeedMod;
            
            if (isOffline)
            {
                SyncChangeAttackSpeedRPC((float)bonusAttackSpeed,attackSpeedMod);
                return;
            }
            photonView.RPC("SyncChangeAttackSpeedRPC",RpcTarget.All,(float)bonusAttackSpeed,attackSpeedMod);
        }

        [PunRPC]
        public void SyncChangeAttackSpeedRPC(float newValue,float mod)
        {
            bonusAttackSpeed = newValue;
            
            if(isMaster) OnChangeAttackSpeed?.Invoke(mod);
            OnChangeAttackSpeedFeedback?.Invoke(mod);
        }

        public event Action<float> OnChangeAttackSpeed;
        public event Action<float> OnChangeAttackSpeedFeedback;
    }
}