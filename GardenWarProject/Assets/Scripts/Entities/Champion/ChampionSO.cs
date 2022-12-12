using Entities.Capacities;
using UnityEngine;

namespace Entities.Champion
{
    [CreateAssetMenu(menuName = "Champion", fileName = "new Champion")]
    public class ChampionSO : ScriptableObject
    {
        [Header("Visual")]
        public Sprite portrait;
        public GameObject championMeshPrefab;
        public int canvasIndex;
        
        [Header("Stats")]
        public float maxHp;
        public float maxMana;
        public float baseDefense;
        public float baseMoveSpeed;
        [Tooltip("Mana per tick")] public float hpRegen;
        [Tooltip("Mana per tick")] public float manaRegen;
        public float attackDamage;
        [Tooltip("Time between attacks")] public double attackSpeed;
        public float attackRange = 10;
        public float baseArmorPenetration;
        public float baseLifesteal;
        
        [Header("Attack")]
        public ActiveCapacitySO attackAbility;
        [HideInInspector] public byte attackAbilityIndex;

        [Header("Abilities")]
        public PassiveCapacitySO[] passiveCapacities;
        [HideInInspector] public byte[] passiveCapacitiesIndexes;
        public ActiveCapacitySO[] activeCapacities; 
        [HideInInspector] public byte[] activeCapacitiesIndexes;

        public void SetIndexes()
        {
            // Attack
            attackAbilityIndex = CapacitySOCollectionManager.GetActiveCapacitySOIndex(attackAbility);
        
            // Passives
            passiveCapacitiesIndexes = new byte[passiveCapacities.Length];
            for (var index = 0; index < passiveCapacities.Length; index++)
            {
                var passiveCapacitySo = passiveCapacities[index];
                passiveCapacitiesIndexes[index] =
                    CapacitySOCollectionManager.GetPassiveCapacitySOIndex(passiveCapacitySo);
            }
            // Actives
            activeCapacitiesIndexes = new byte[activeCapacities.Length];
            for (var index = 0; index < activeCapacitiesIndexes.Length; index++)
            {
                var activeCapacitySo = activeCapacities[index];
                activeCapacitiesIndexes[index] =
                    CapacitySOCollectionManager.GetActiveCapacitySOIndex(activeCapacitySo);
            }
        }
    }
}


