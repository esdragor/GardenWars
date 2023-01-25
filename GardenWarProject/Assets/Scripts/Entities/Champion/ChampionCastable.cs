using System;
using System.Collections.Generic;
using System.Linq;
using Entities.Capacities;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Random = UnityEngine.Random;

namespace Entities.Champion
{
    public partial class Champion : ICastable
    {
        [Header("Recall")]
        [SerializeField] private RecallSO recall;

        [Header("Candy Buff")]
        [SerializeField] private ConsumeCandySO consumeCandy;

        public byte recallAbilityIndex => recall.indexInCollection;
        public byte consumeAbilityIndex => consumeCandy.indexInCollection;

        [Header("Cooldown")]
        public float cooldownReduction;
        public float currentCd => 1 - cooldownReduction;

        [Header("Range Indicator")]
        [SerializeField] private GameObject areaIndicatorPrefab;
        [SerializeField] private Material maxRangeMat;
        [Header("Area Indicator")]
        [SerializeField] private Material areaMat;
        [SerializeField] private float indicatorHeight = 0.01f;
        [Header("Skillshot Indicator")]
        [SerializeField] private GameObject skillShotIndicatorGo;
        private Transform skillShotIndicatorTr;
        [Header("Text Indicator")]
        [SerializeField] private TextMeshProUGUI textIndicatorPrefab;
        private TextMeshProUGUI textIndicator;
        private GameObject textIndicatorGo;
        private RectTransform textIndicatorTr;
        
        private GameObject maxRangeIndicatorGo;
        private GameObject areaIndicatorGo;
        private Transform maxRangeIndicatorTr;
        
        private Transform areaIndicatorTr;
        [SerializeField] private float rangeScaleFactor = 0.25f;
        
        public byte[] abilitiesIndexes = new byte[3];
        public Dictionary<byte, CastingAbility> capacityDict { get; private set; } = new Dictionary<byte, CastingAbility>();

        public int targetedEntities;
        public Vector3 targetedPositions;

        [Header("Upgrades")]
        [SerializeField] private int upgradeCount;
        [SerializeField] private GameObject upgradeFx;

        [Header("Candy")]
        [SerializeField] private GameObject candyPickUpFx;
        public int upgrades => upgradeCount;

        public byte throwAbilityIndex { get; private set; }

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

        public void ChangeActiveAbility(int index,byte abilityId)
        {
            if (isOffline)
            {
                SyncChangeActiveAbility(index, abilityId);
                return;
            }
            photonView.RPC("SyncChangeActiveAbility",RpcTarget.All,index,abilityId);
        }

        [PunRPC]
        private void SyncChangeActiveAbility(int index,byte abilityIndex)
        {
            if(index<0 || index >=3) return;
            abilitiesIndexes[index] = abilityIndex;

            var capacity = SetupAbility(abilityIndex);
            
            OnChangedAbilityFeedback?.Invoke(index,capacity);
        }

        public event Action<int, ActiveCapacity> OnChangedAbilityFeedback;

        private ActiveCapacity SetupAbility(byte index)
        {
            var newCapacity = new CastingAbility
            {
                isCasting = false,
                capacity = CapacitySOCollectionManager.CreateActiveCapacity(index,this)
            };

            if (capacityDict.ContainsKey(index)) capacityDict[index] = newCapacity;
            else capacityDict.Add(index,newCapacity);

            return newCapacity.capacity;
        }

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
            
            Debug.Log($"Trying to upgrade capacity at index {index}");
            Debug.Log($"It's {capacityDict[abilitiesIndexes[index]].capacity}");
            if(!capacityDict[abilitiesIndexes[index]].capacity.canBeUpgraded) return;
            
            DecreaseUpgradeCount();
            
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
            if(!isFighter) return;
            var capacity = capacityDict[abilitiesIndexes[index]];
            capacity.capacity.Upgrade();
            OnAbilityUpgraded?.Invoke();
        }

        public event Action OnAbilityUpgraded;


        public void IncreaseUpgradeCount()
        {
            if(!isMaster) return;

            upgradeCount++;

            if (isOffline)
            {
                SyncIncreaseUpgradeCountRPC(upgradeCount);
                return;
            }
            
            photonView.RPC("SyncIncreaseUpgradeCountRPC",RpcTarget.All,upgradeCount);
        }

        [PunRPC]
        private void SyncIncreaseUpgradeCountRPC(int value)
        {
            upgradeCount = value;
            
            upgradeFx.SetActive(upgradeCount > 0);
            
            OnUpgradeCountIncreased?.Invoke();
        }

        public event Action OnUpgradeCountIncreased; 
        
        public void DecreaseUpgradeCount()
        {
            if(!isMaster) return;

            upgradeCount--;

            if (isOffline)
            {
                SyncDecreaseUpgradeCountRPC(upgradeCount);
                return;
            }
            
            photonView.RPC("SyncDecreaseUpgradeCountRPC",RpcTarget.All,upgradeCount);
        }

        [PunRPC]
        private void SyncDecreaseUpgradeCountRPC(int value)
        {
            upgradeCount = value;
            
            upgradeFx.SetActive(upgradeCount > 0);
            
            OnUpgradeCountDecreased?.Invoke();
        }

        public event Action OnUpgradeCountDecreased; 

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
        
        public void ShowAreaIndicator(Vector3 pos,float size)
        {
            if(!areaIndicatorGo.activeSelf) areaIndicatorGo.SetActive(true);
            areaIndicatorTr.localScale = size * rangeScaleFactor * Vector3.one;
            pos.y = position.y + indicatorHeight;
            areaIndicatorTr.transform.position = pos;
        }

        public void HideAreaIndicator()
        {
            areaIndicatorGo.SetActive(false);
        }

        public void ShowSkillShotIndicator(Vector3 pos, float range)
        {
            if (pos == position)
            {
                skillShotIndicatorGo.SetActive(false);
                return;
            }
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

        public void ShowTextIndicator(Vector3 pos, string text)
        {
            textIndicator.text = text;
            textIndicatorTr.position = pos;
            textIndicatorGo.SetActive(true);
        }

        public void HideTextIndicator()
        {
            textIndicator.text = string.Empty;
            textIndicatorGo.SetActive(false);
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
            HideTextIndicator();
        }

        public void ChangeCooldownReduction(float amount)
        {
            cooldownReduction += amount;

            if (isOffline)
            {
                SyncChangeCooldownReductionRPC(cooldownReduction);
                return;
            }

            photonView.RPC("SyncChangeCooldownReductionRPC",RpcTarget.All,cooldownReduction);
        }

        [PunRPC]
        private void SyncChangeCooldownReductionRPC(float amount)
        {
            cooldownReduction = amount;
        }

        [SerializeField] private ProjectileOnCollideEffect candyProjectile;
        private List<Vector3> currentCandyOnMap = new List<Vector3>();
        public void SpawnCandy(int amount,List<Transform> transforms)
        {
            var list = transforms.ToList();
            var tr = list[Random.Range(0, transforms.Count)];
            while (currentCandyOnMap.Contains(tr.position))
            {
                list.Remove(tr);
                if (list.Count <= 0) return;

                tr = transforms[Random.Range(0, transforms.Count)];
            }
            
            photonView.RPC("SyncSpawnCandyRPC", RpcTarget.All, amount,tr.position);
        }

        [PunRPC]
        private void SyncSpawnCandyRPC(int amount, Vector3 pos)
        {
            var projectile = LocalPoolManager.PoolInstantiate(candyProjectile, pos, Quaternion.identity);
            currentCandyOnMap.Add(pos);

            projectile.OnEntityCollideFeedback += GiveCandy;

            void GiveCandy(Entity entity)
            {
                if (!(entity is Champion champ)) return;

                if (isMaster)
                {
                    if(currentCandyOnMap.Contains(pos)) currentCandyOnMap.Remove(pos);
                    champ.IncreaseCurrentCandyRPC(amount);
                }

                projectile.OnEntityCollideFeedback -= GiveCandy;
                
                var go = LocalPoolManager.PoolInstantiate(candyPickUpFx, projectile.transform.position, Quaternion.Euler(-90, 0, 0));
                go.SetActive(false);
                go.SetActive(entity.isVisible);
                    
                projectile.DestroyProjectile(true);
            }
        }
        
    }
}