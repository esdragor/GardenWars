using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class TowerBT : Tree
    {
        [SerializeField] private LayerMask enemyMask;
        [SerializeField] private Tower entity;
        [SerializeField] public Transform Poussin;
        private float atkDelay => entity.GetAttackSpeed();
        
        protected override Node InitTree()
        {
            origin = new Sequence(new List<Node>
            {
                new CheckEnemyInPOVRange(this, entity,enemyMask, entity.GetFOWViewRange(), entity.GetFOWViewRange()), 
                new TaskAttack(this, entity, Poussin, entity.activeTowerAutoSO.indexInCollection, atkDelay, null, "AttackTower")
            });
            return origin;
        }
    }
}
