using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Entities.Capacities
{
    [CreateAssetMenu(menuName = "Capacity/ActiveCapacitySO/SkillShot", fileName = "new SkillShot")]
    public class SkillShotSO : ActiveCapacitySO
    {
        public GameObject projectile;
        public float projectileSpeed = 1f;
        
        public override Type AssociatedType()
        {
            return typeof(SkillShot);
        }
    }
    
    public class SkillShot : ActiveCapacity
    {
        private SkillShotSO so => (SkillShotSO) AssociatedActiveCapacitySO();
        protected override bool AdditionalCastConditions(int targetsEntityIndexes, Vector3 targetPositions)
        {
            return true;
        }

        protected override void Press(int targetsEntityIndexes, Vector3 targetPositions)
        {
            
        }

        protected override void PressFeedback(int targetsEntityIndexes, Vector3 targetPositions)
        {
            
        }

        protected override void Hold(int targetsEntityIndexes, Vector3 targetPositions)
        {
            
        }

        protected override void HoldFeedback(int targetsEntityIndexes, Vector3 targetPositions)
        {
            
        }

        protected override void Release(int targetsEntityIndexes, Vector3 targetPositions)
        {
            
        }

        protected override void ReleaseFeedback(int targetEntityIndex, Vector3 targetPositions)
        {
            Object.Instantiate(so.projectile,casterPos+champion.forward,champion.rotation);
        }
    }
}


