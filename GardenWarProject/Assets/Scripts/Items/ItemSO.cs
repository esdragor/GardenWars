using UnityEngine;
using System;
using Entities.Capacities;
using UnityEngine.Serialization;


namespace Entities.Inventory
{
    //Asset Menu Syntax :
    //[CreateAssetMenu(menuName = "ItemS", fileName = "new ItemSO")]
    public abstract class ItemSO : ScriptableObject
    {
        [Tooltip("GD Name")] public string itemName;
        [TextArea(4, 4)] [Tooltip("Description of the item")] public string description;
        public Sprite sprite;

        [Header("Gameplay")]
        public bool consumable;
        [Tooltip("In seconds")] public float activationCooldown = 0;
        public PassiveCapacitySO[] passiveCapacities;
        [HideInInspector] public byte[] passiveCapacitiesIndexes;
        public ActiveCapacitySO[] activeCapacities;
        [HideInInspector] public byte[] activeCapacitiesIndexes;
        [Header("Minion")]
        public PassiveCapacitySO[] MinionPassiveCapacities = new PassiveCapacitySO[0];
        [HideInInspector] public byte[] MinionPassiveCapacitiesIndexes = new byte[0];
        public ActiveCapacitySO[] MinionActiveCapacities = new ActiveCapacitySO[0];
        [HideInInspector] public byte[] MinionActiveCapacitiesIndexes = new byte[0];
        

        /// <returns>the type of Item associated with this ItemSO</returns>
        public abstract Type AssociatedType();
        [HideInInspector] public byte indexInCollection;
        
        public void SetIndexes(byte indexInColle)
        {
            indexInCollection = indexInColle;
            // Passives
            passiveCapacitiesIndexes = new byte[passiveCapacities.Length];
            for (var index = 0; index < passiveCapacities.Length; index++)
            {
                var passiveCapacitySo = passiveCapacities[index];
                passiveCapacitiesIndexes[index] =
                    CapacitySOCollectionManager.GetPassiveCapacitySOIndex(passiveCapacitySo);
            }
            if (MinionPassiveCapacities != null)
            MinionPassiveCapacitiesIndexes = new byte[ MinionPassiveCapacities.Length];
            for (var index = 0; index <  MinionPassiveCapacities.Length; index++)
            {
                var passiveCapacitySo =  MinionPassiveCapacities[index];
                MinionPassiveCapacitiesIndexes[index] =
                    CapacitySOCollectionManager.GetPassiveCapacitySOIndex(passiveCapacitySo);
            }
            // Actives
            activeCapacitiesIndexes = new byte[activeCapacities.Length];
            for (var index = 0; index < activeCapacities.Length; index++)
            {
                var activeCapacitySo = activeCapacities[index];
                activeCapacitiesIndexes[index] =
                    CapacitySOCollectionManager.GetActiveCapacitySOIndex(activeCapacitySo);
            }
            MinionActiveCapacitiesIndexes = new byte[MinionActiveCapacities.Length];
            for (var index = 0; index < MinionActiveCapacities.Length; index++)
            {
                var activeCapacitySo = MinionActiveCapacities[index];
                MinionActiveCapacitiesIndexes[index] =
                    CapacitySOCollectionManager.GetActiveCapacitySOIndex(activeCapacitySo);
            }
        }
    }
}
