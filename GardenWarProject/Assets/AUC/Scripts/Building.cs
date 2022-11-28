using System.Collections;
using System.Collections.Generic;
using Entities;
using UnityEngine;

namespace Entities
{
    public class Building : Entity
    {
        [Space]
        [Header("Life Building settings")]
        public bool isAlive;
        public float maxHealth;
        public float currentHealth;

        protected override void OnStart()
        {
            currentHealth = maxHealth;
        }

        public override void OnInstantiated()
        {
            
        }
    }
}

