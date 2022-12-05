using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class TowerBT : Tree
    {
        private Node origin;
        
        [SerializeField] private LayerMask enemyMask;
        [SerializeField] private Tower entity;
        [SerializeField] private float AtkRange;
        [SerializeField] private float FOVRange;
        [SerializeField] private float atkDelay = 15;
        
        protected override Node InitTree()
        {
            origin = new Sequence(new List<Node>
                {
                    new CheckEnemyInPOVRange(origin, entity,enemyMask, FOVRange),
                    new TaskAttack(origin, entity, atkDelay, true),
                });

            return origin;
        }
    }
}
