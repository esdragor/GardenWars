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
        [SerializeField] private Transform Poussin;
        private float atkDelay => entity.GetAttackSpeed();
        
        protected override Node InitTree()
        {
            origin = new Sequence(new List<Node>
            {
                new CheckEnemyInPOVRange(origin, entity,enemyMask, AtkRange), 
                new TaskAttack(origin, entity, Poussin, entity.activeTowerAutoSO.indexInCollection, atkDelay, null)
            });
            return origin;
        }
    }
}
