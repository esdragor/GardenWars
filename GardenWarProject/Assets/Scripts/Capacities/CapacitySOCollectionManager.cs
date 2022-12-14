using System;
using System.Collections.Generic;
using UnityEngine;

namespace Entities.Capacities
{
    public class CapacitySOCollectionManager : MonoBehaviour
    {
        public static CapacitySOCollectionManager Instance;

        [SerializeField] private ActiveCapacitySO scavengerThrowCapacitySo;
        [SerializeField] private ActiveCapacitySO fighterThrowCapacitySo;

        [SerializeField] private List<ActiveCapacitySO> allActiveCapacities = new List<ActiveCapacitySO>();
        
        [SerializeField] private List<PassiveCapacitySO> allPassiveCapacitiesSo = new List<PassiveCapacitySO>();

        private void Awake()
        {
            Debug.Log("Setting Instance");
            if (Instance != null && Instance != this)
            {
                DestroyImmediate(gameObject);
                return;
            }

            Instance = this;
            

        }

        public void SetIndexes()
        {
            Debug.Log("Setting indexes");
            for (byte i = 0; i < allActiveCapacities.Count; i++)
            {
                allActiveCapacities[i].indexInCollection = i;
            }
            for (byte i = 0; i < allPassiveCapacitiesSo.Count; i++)
            {
                allPassiveCapacitiesSo[i].indexInCollection = i;
            }

            foreach (var active in allActiveCapacities)
            {
                Debug.Log($"Active: {active.name} - {active.indexInCollection}");
            }
        }

        //=========================ACTIVE=====================================

        public static byte GetActiveCapacitySOIndex(ActiveCapacitySO so)
        {
            return (byte)Instance.allActiveCapacities.IndexOf(so);
        }

        public static ActiveCapacity CreateActiveCapacity(byte soIndex,Entity caster)
        {
            var active = (ActiveCapacity) Activator.CreateInstance(Instance.allActiveCapacities[soIndex].AssociatedType());
            active.indexOfSOInCollection = soIndex;
            active.caster = caster;
            return active;
        }

        /// <summary>
        /// Return Active Capacity by his Index in allActiveCapacities
        /// </summary>
        /// <param name="index">Capacity Index</param>
        /// <returns></returns>
        public static ActiveCapacitySO GetActiveCapacitySOByIndex(byte index)
        {
            return Instance.allActiveCapacities[index];
        }

        //=========================PASSIF=====================================

        public static byte GetPassiveCapacitySOIndex(PassiveCapacitySO so)
        {
            return (byte)Instance.allPassiveCapacitiesSo.IndexOf(so);
        }

        public PassiveCapacity CreatePassiveCapacity(byte soIndex,Entity entity)
        {
            Debug.Log($"Trying to create passive capacity of so at {soIndex}");
            if(soIndex>= allPassiveCapacitiesSo.Count) return null;
            var so = allPassiveCapacitiesSo[soIndex];
            PassiveCapacity capacity;
            if (so.stackable)
            {
                capacity = entity.GetPassiveCapacityBySOIndex(soIndex);
                if (capacity != null)
                {
                    capacity.stackable = so.stackable;
                    return capacity;
                }
            }
            capacity = (PassiveCapacity) Activator.CreateInstance(allPassiveCapacitiesSo[soIndex].AssociatedType());
            capacity.stackable = so.stackable;
            capacity.indexOfSo = soIndex;
            
            return capacity;
        }

        /// <summary>
        /// Return Passive Capacity by his Index in allPassiveCapacities
        /// </summary>
        /// <param name="index">Capacity Index</param>
        /// <returns></returns>
        public PassiveCapacitySO GetPassiveCapacitySOByIndex(byte index)
        {
            return allPassiveCapacitiesSo[index];
        }

        public static byte GetThrowAbilityIndex(Enums.ChampionRole role)
        {
            return role == Enums.ChampionRole.Fighter ?  Instance.fighterThrowCapacitySo.indexInCollection : Instance.scavengerThrowCapacitySo.indexInCollection;
        }
    }
}