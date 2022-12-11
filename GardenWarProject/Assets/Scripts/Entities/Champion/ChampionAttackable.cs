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
        public double attackSpeed;
        public float attackRange;

        public bool CanAttack()
        {
            return canAttack;
        }

        public void RequestSetCanAttack(bool value) { }

        [PunRPC]
        public void SyncSetCanAttackRPC(bool value) { }

        [PunRPC]
        public void SetCanAttackRPC(bool value) { }

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
            return;

            /*
            lastCapacityIndex = attackIndex;
            lastTargetedEntities = targetedEntities;
            lastTargetedPositions = targetedPositions;
            
            
            lastCapacity = CapacitySOCollectionManager.CreateActiveCapacity(attackIndex,this);
            lastCapacitySO = CapacitySOCollectionManager.GetActiveCapacitySOByIndex(attackIndex);
            var targetEntity = EntityCollectionManager.GetEntityByIndex(targetedEntities[0]);

            if (lastCapacity.CanCast(targetedEntities, targetedPositions))
            {
                if (lastCapacitySO.shootType != Enums.CapacityShootType.Skillshot)
                {
                    bool isTargetEntity = lastCapacitySO.shootType == Enums.CapacityShootType.TargetEntity;
                    Vector3 position = isTargetEntity
                        ? EntityCollectionManager.GetEntityByIndex(targetedEntities[0]).transform.position
                        : targetedPositions[0];

                    if (!lastCapacity.isInRange(position))
                    {
                        Debug.Log("Not in range");
                        if (isTargetEntity) SendFollowEntity(targetedEntities[0], lastCapacitySO.maxRange);
                        else agent.SetDestination(position);
                    }
                    else
                    {
                        Debug.Log("Attack in Range");
                        agent.SetDestination(transform.position);
                        OnAttack?.Invoke(attackIndex, targetedEntities, targetedPositions);
                        photonView.RPC("SyncAttackRPC", RpcTarget.All, attackIndex, targetedEntities,
                            targetedPositions);

                    }
                }
                else
                {
                    agent.SetDestination(transform.position);
                    OnAttack?.Invoke(attackIndex, targetedEntities, targetedPositions);
                    photonView.RPC("SyncAttackRPC", RpcTarget.All, attackIndex, targetedEntities, targetedPositions);
                }
            }
            else
            {
                //Cant Attack FeedBack
            }
            */
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
                var newCapacity = new CastingAbility
                {
                    isCasting = false,
                    capacity = CapacitySOCollectionManager.CreateActiveCapacity(attackIndex,this)
                };
                capacityDict.Add(attackIndex,newCapacity);
            }
            
            Debug.Log($"Trying to attack Entity {targetedEntities}");
            
            capacityDict[attackIndex].capacity.OnRelease(targetedEntities,targetedPositions);
            if(isMaster) OnAttack?.Invoke(attackIndex,targetedEntities,targetedPositions);
            OnAttackFeedback?.Invoke(attackIndex,targetedEntities,targetedPositions);
        }

        public event GlobalDelegates.ByteIntArrayVector3ArrayDelegate OnAttack;
        public event GlobalDelegates.ByteIntArrayVector3ArrayDelegate OnAttackFeedback;
    }
}