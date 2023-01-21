using System;
using System.Collections.Generic;
using UnityEngine;

namespace Entities.Capacities
{
//Asset Menu Syntax :
//[CreateAssetMenu(menuName = "Capacity/ActiveCapacitySO", fileName = "new ActiveCapacitySO")]
    public abstract class ActiveCapacitySO : ScriptableObject
    {
        [Tooltip("GD Name")] public string capacityName;

        [Tooltip("Capacity Icon")] public Sprite icon;

        [TextArea(4, 4)] [Tooltip("Description of the capacity")]
        public string description;
        public string description1;
        public string description2;

        public uint maxLevel = 3;

        [Tooltip("In Seconds")] public float cooldown;
        
        [Tooltip("In Seconds")] public float castTime;
        
        [Tooltip("Maximum range")] public float maxRange;

        [Tooltip("All types of the capacity")] public List<Enums.CapacityType> types;

        public Enums.CapacityShootType shootType;
        
        public bool showMaxRangeIfSkillShot;
        public bool overrideDefaultIndicator;
        
        public abstract Type AssociatedType();

        [HideInInspector] public byte indexInCollection;
    }
}