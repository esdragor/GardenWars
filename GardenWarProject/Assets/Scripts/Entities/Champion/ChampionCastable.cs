using System;
using System.Collections.Generic;
using System.Linq;
using Entities.Capacities;
using UnityEngine;
using Photon.Pun;

namespace Entities.Champion
{
    public partial class Champion : ICastable
    {
        [Header("Recall")]
        [SerializeField] private RecallSO recall;

        public byte recallAbilityIndex => recall.indexInCollection;

        [Header("Range Indicator")]
        [SerializeField] private GameObject areaIndicatorPrefab;

        [SerializeField] private Material maxRangeMat;
        [SerializeField] private Material areaMat;
        [SerializeField] private float indicatorHeight = 0.01f;

        [SerializeField] private GameObject skillShotIndicatorGo;
        private Transform skillShotIndicatorTr;
        
        private GameObject maxRangeIndicatorGo;
        private GameObject areaIndicatorGo;
        private Transform maxRangeIndicatorTr;
        
        private Transform areaIndicatorTr;
        [SerializeField] private float rangeScaleFactor = 0.25f;
        
        public byte[] abilitiesIndexes = new byte[3];
        public Dictionary<byte, CastingAbility> capacityDict { get; private set; } = new Dictionary<byte, CastingAbility>();

        public int targetedEntities;
        public Vector3 targetedPositions;

        private int upgradeCount = 0;

        public class CastingAbility
        {
            public bool isCasting = false;
            public ActiveCapacity capacity;
        }

        public bool canCast;
        
        public bool CanCast()
        {
            return canCast;
        }
        
        public void RequestSetCanCast(bool value)
        {
            if (isMaster)
            {
                SetCanCastRPC(value);
                return;
            }
            photonView.RPC("SetCanCastRPC", RpcTarget.MasterClient, value);
        }
        
        [PunRPC]
        public void SetCanCastRPC(bool value)
        {
            canCast = value;
            OnSetCanCast?.Invoke(value);
            if (isOffline)
            {
                SyncSetCanCastRPC(value);
                return;
            }
            photonView.RPC("SyncSetCanCastRPC", RpcTarget.All, value);
        }

        [PunRPC]
        public void SyncSetCanCastRPC(bool value)
        {
            canCast = value;
            OnSetCanCastFeedback?.Invoke(value);
        }
        
        public event GlobalDelegates.BoolDelegate OnSetCanCast;
        public event GlobalDelegates.BoolDelegate OnSetCanCastFeedback;

        public void RequestCast(byte capacityIndex, int targetedEntities, Vector3 targetedPositions) { }
        
        public void CastRPC(byte capacityIndex, int targetedEntities, Vector3 targetedPositions) { }
        
        public void SyncCastRPC(byte capacityIndex, int targetedEntities, Vector3 targetedPositions) { }
        
        public event GlobalDelegates.ByteIntArrayVector3ArrayDelegate OnCast;
        public event GlobalDelegates.ByteIntArrayVector3ArrayDelegate OnCastFeedback;
        
        
        public void RequestOnCastCapacity(byte capacityIndex)
        {
            if(isMaster)
            {
                OnCastCapacityRPC(capacityIndex,targetedEntities,targetedPositions);
                return;
            }
            photonView.RPC("OnCastCapacityRPC",RpcTarget.MasterClient,capacityIndex,targetedEntities,targetedPositions);
        }

        [PunRPC]
        private void OnCastCapacityRPC(byte capacityIndex, int newTargetedEntities, Vector3 newTargetedPositions)
        {
            if(!canCast) return;
            if (isOffline)
            {
                SyncOnCastCapacityRPC(capacityIndex, newTargetedEntities, newTargetedPositions);
                return;
            }
            photonView.RPC("SyncOnCastCapacityRPC",RpcTarget.All,capacityIndex,newTargetedEntities,newTargetedPositions);
        }
        
        [PunRPC]
        private void SyncOnCastCapacityRPC(byte capacityIndex, int newTargetedEntities, Vector3 newTargetedPositions)
        {
            targetedEntities = newTargetedEntities;
            targetedPositions = newTargetedPositions;
            if (!capacityDict.ContainsKey(capacityIndex))
            {
                var newCapacity = new CastingAbility
                {
                    isCasting = true,
                    capacity = CapacitySOCollectionManager.CreateActiveCapacity(capacityIndex,this)
                };
                capacityDict.Add(capacityIndex,newCapacity);
            }

            capacityDict[capacityIndex].isCasting = true;
            
            capacityDict[capacityIndex].capacity.OnPress(targetedEntities,targetedPositions);
        }

        public void  ChangeActiveAbility(int index,byte abilityId)
        {
            if (isOffline)
            {
                SyncChangeActiveAbility(index, abilityId);
                return;
            }
            photonView.RPC("SyncChangeActiveAbility",RpcTarget.All,index,abilityId);
        }
        
        [PunRPC]
        private void SyncChangeActiveAbility(int index,byte abilityId)
        {
            if(index<0 || index >=3) return;
            abilitiesIndexes[index] = abilityId;
            
            var newCapacity = new CastingAbility
            {
                isCasting = false,
                capacity = CapacitySOCollectionManager.CreateActiveCapacity(abilityId,this)
            };

            if (capacityDict.ContainsKey(abilityId)) capacityDict[abilityId] = newCapacity;
            else capacityDict.Add(abilityId,newCapacity);
            
            OnChangedAbilityFeedback?.Invoke(index,newCapacity.capacity);
        }

        public event Action<int, ActiveCapacity> OnChangedAbilityFeedback;

        private void CastHeldCapacities()
        {
            if(!canCast) return;
            foreach (var ability in capacityDict.Values.Where(ability => ability.isCasting))
            {
                ability.capacity.OnHold(targetedEntities,targetedPositions);
            }
        }

        public void RequestOnReleaseCapacity(byte capacityIndex)
        {
            if(isMaster)
            {
                OnReleaseCapacityRPC(capacityIndex,targetedEntities,targetedPositions);
                return;
            }
            photonView.RPC("OnReleaseCapacityRPC",RpcTarget.MasterClient,capacityIndex,targetedEntities,targetedPositions);
        }

        [PunRPC]
        private void OnReleaseCapacityRPC(byte capacityIndex, int newTargetedEntities, Vector3 newTargetedPositions)
        {
            if(!canCast) return;
            if (isOffline)
            {
                SyncOnReleaseCapacityRPC(capacityIndex, newTargetedEntities, newTargetedPositions);
                return;
            }
            photonView.RPC("SyncOnReleaseCapacityRPC",RpcTarget.All,capacityIndex,newTargetedEntities,newTargetedPositions);
        }
        
        [PunRPC]
        private void SyncOnReleaseCapacityRPC(byte capacityIndex, int newTargetedEntities, Vector3 newTargetedPositions)
        {
            targetedEntities = newTargetedEntities;
            targetedPositions = newTargetedPositions;
            if (!capacityDict.ContainsKey(capacityIndex))
            {
                var newCapacity = new CastingAbility
                {
                    isCasting = true,
                    capacity = CapacitySOCollectionManager.CreateActiveCapacity(capacityIndex,this)
                };
                capacityDict.Add(capacityIndex,newCapacity);
            }

            if(!capacityDict[capacityIndex].isCasting) return;
            capacityDict[capacityIndex].isCasting = false;

            capacityDict[capacityIndex].capacity.OnRelease(targetedEntities,targetedPositions);
            if(isMaster) OnCast?.Invoke(capacityIndex,targetedEntities,targetedPositions);
            
            OnCastFeedback?.Invoke(capacityIndex,targetedEntities,targetedPositions);
        }

        public void RequestUpgrade(int index)
        {
            if (isMaster)
            {
                UpgradeRPC(index);
                return;
            }
            photonView.RPC("UpgradeRPC", RpcTarget.MasterClient, index);
        }
        
        [PunRPC]
        public void UpgradeRPC(int index)
        {
            if(upgradeCount <= 0) return;
            
            if(!capacityDict[abilitiesIndexes[index]].capacity.canBeUpgraded) return;
            
            if (isOffline)
            {
                SyncUpgradeRPC(index);
                return;
            }
            photonView.RPC("SyncUpgradeRPC",RpcTarget.All,index);
        }
        
        [PunRPC]
        private void SyncUpgradeRPC(int index)
        {
            Debug.Log($"Upgrade capacity at index {index} ({capacityDict[abilitiesIndexes[index]].capacity.AssociatedActiveCapacitySO().capacityName}) (now level {capacityDict[abilitiesIndexes[index]].capacity.level})");
            upgradeCount--;
            capacityDict[abilitiesIndexes[index]].capacity.level++;
            OnAbilityUpgraded?.Invoke();
        }

        public event Action OnAbilityUpgraded;

        public void ShowMaxRangeIndicator(float range)
        {
            maxRangeIndicatorTr.localScale = range * rangeScaleFactor * Vector3.one;
            if (maxRangeIndicatorGo.activeSelf) return;
            maxRangeIndicatorGo.SetActive(true);
            
        }

        public void HideMaxRangeIndicator()
        {
            maxRangeIndicatorGo.SetActive(false);
        }
        
        public void ShowAreaIndicator(Vector3 pos,float range)
        {
            if(!areaIndicatorGo.activeSelf) areaIndicatorGo.SetActive(true);
            areaIndicatorTr.localScale = range * rangeScaleFactor * Vector3.one;
            pos.y = position.y + indicatorHeight;
            areaIndicatorTr.transform.position = pos;
        }

        public void HideAreaIndicator()
        {
            areaIndicatorGo.SetActive(false);
        }

        public void ShowSkillShotIndicator(Vector3 pos, float range)
        {
            var dir = (pos - position).normalized;

            skillShotIndicatorTr.localRotation = Quaternion.LookRotation(dir);
            
            dir = position + dir * range;
            dir.y = position.y + indicatorHeight;
            skillShotIndicatorTr.position = dir;
            
            skillShotIndicatorGo.SetActive(true);
        }

        public void HideSkillShotIndicator()
        {
            skillShotIndicatorTr.localPosition = Vector3.zero;
            skillShotIndicatorGo.SetActive(false);
        }

        public void CancelCast()
        {
            if (!capacityDict.Values.Any(ability => ability.isCasting)) return;
            
            foreach (var ability in capacityDict.Values.Where(ability => ability.isCasting))
            {
                ability.isCasting = false;
            }
            
            HideSkillShotIndicator();
            HideMaxRangeIndicator();
            HideAreaIndicator();
        }
        
        
    }
}