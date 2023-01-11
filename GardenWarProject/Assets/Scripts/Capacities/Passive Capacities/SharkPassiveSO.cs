using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Entities.Capacities
{
    [CreateAssetMenu(menuName = "Capacity/PassiveCapacitySO/SharkPassive", fileName = "SharkPassive")]
    public class SharkPassiveSO : PassiveCapacitySO
    {
        public double timeUntilBorrow = 3;
        public float borrowDamage = 10f;
        public double knockUpDuration = 1;
        public float knockUpDamage = 20f;
        public ProjectileOnCollideEffect aileron;
        public StunPassiveSO stunPassive;
        public ParticleSystem UnborrowBlue;
        public ParticleSystem UnborrowRed;

        public override Type AssociatedType()
        {
            return typeof(SharkPassive);
        }
    }

    public class SharkPassive : PassiveCapacity
    {
        public bool borrowed;
        public float bonusDamage = 0;
        
        private SharkPassiveSO so => (SharkPassiveSO) AssociatedPassiveCapacitySO();

        private ProjectileOnCollideEffect aileron;
        private GameObject aileronGo;
        
        private double timeUnBorrowed;
        private ParticleSystem Unborrow;
        private GameObject UnborrowGO;

        public override PassiveCapacitySO AssociatedPassiveCapacitySO()
        {
            return CapacitySOCollectionManager.Instance.GetPassiveCapacitySOByIndex(indexOfSo);
        }

        protected override void OnAddedEffects(Entity target)
        {
            timeUnBorrowed = 0;
            borrowed = false;
            bonusDamage = 0;
            
            aileron = Object.Instantiate(so.aileron, champion.rotateParent.position+Vector3.up * 0.75f, champion.rotation,
                champion.rotateParent);
            aileronGo = aileron.gameObject;
            aileronGo.SetActive(false);

            aileron.OnEntityCollide += EntityCollide;
            
            gsm.OnUpdate += IncreaseTimeUnBorrowed;
            
            champion.OnAttack += ResetTimer;
            champion.OnAttack += UnBorrow;
            
            champion.OnDie += UnBorrow;
            champion.OnDie += ResetTimer;
        }

        protected override void OnAddedFeedbackEffects(Entity target)
        {
            if (Entity.isMaster) return;

            aileron = Object.Instantiate(so.aileron, champion.rotateParent.position+Vector3.up * 0.75f, champion.rotation,
                champion.rotateParent);
            aileronGo = aileron.gameObject;
            aileronGo.SetActive(false);

            
            timeUnBorrowed = 0;
            borrowed = false;

            gsm.OnUpdateFeedback += IncreaseTimeUnBorrowed;
            
            champion.OnAttackFeedback += ResetTimer;
            champion.OnAttackFeedback += UnBorrow;
            
            champion.OnDieFeedback += ResetTimer;
        }
        
        private void IncreaseTimeUnBorrowed()
        {
            if(borrowed) return;
            
            timeUnBorrowed += Time.deltaTime;
            if (timeUnBorrowed >= so.timeUntilBorrow)
            {
                Borrow();
            }
        }

        public void Borrow()
        {
            if(borrowed) return;
            
            //champion.SetCanBeTargetedRPC(false);
            
            champion.rotateParent.localPosition = Vector3.up * -0.75f;
            
            if (Entity.isMaster)
            {
                champion.OnAttack += StunTarget;
            }

            borrowed = true;
            
            aileronGo.SetActive(true);
            aileronGo.GetComponent<SharkPassiveManager>().EnableFXShot(champion.team);
        }

        private void StunTarget(byte _,int targetId,Vector3 __)
        {
            var target = EntityCollectionManager.GetEntityByIndex(targetId);
            if(target == null) return;
            if(!champion.GetEnemyTeams().Contains(target.team)) return;
            
            target.AddPassiveCapacityRPC(so.stunPassive.indexInCollection);
            
            champion.OnAttack -= StunTarget;
        }
        
        private void UnBorrow(byte _,int targetId,Vector3 ___)
        {
            var target = EntityCollectionManager.GetEntityByIndex(targetId);
            if(target == null) return;
            if(!champion.GetEnemyTeams().Contains(target.team)) return;
            
            if(!borrowed) return;
            
            //champion.SetCanBeTargetedRPC(true);

            if (Entity.isMaster)
            {
                champion.OnAttack -= StunTarget;
            }
            
            champion.rotateParent.localPosition = Vector3.zero;
            
            borrowed = false;
            if (!Unborrow)
            {
                Unborrow = champion.team == gsm.GetPlayerTeam() ? so.UnborrowBlue : so.UnborrowRed;
                UnborrowGO = Object.Instantiate(Unborrow, champion.championMesh.transform).gameObject;
            }
            UnborrowGO.SetActive(false);
            UnborrowGO.SetActive(true);

            aileronGo.SetActive(false);
        }
        
        private void UnBorrow(int _)
        {
            UnBorrow(0,0,Vector3.zero);
        }
        
        private void ResetTimer(byte _,int __,Vector3 ___)
        {
            timeUnBorrowed = 0;
            bonusDamage = 0;
        }
        
        private void ResetTimer(int _)
        {
            ResetTimer(0,0,Vector3.zero);
        }

        private void EntityCollide(Entity entity)
        {
            if (!champion.GetEnemyTeams().Contains(entity.team)) return;
            
            var lifeable = entity.GetComponent<IActiveLifeable>();
            
            lifeable.DecreaseCurrentHpRPC(so.borrowDamage+bonusDamage, champion.entityIndex);
        }

        protected override void OnRemovedEffects(Entity target)
        {
            aileron.OnEntityCollide -= EntityCollide;
            
            gsm.OnUpdate -= IncreaseTimeUnBorrowed;
            
            champion.OnAttack -= ResetTimer;
            champion.OnAttack -= UnBorrow;
            
            champion.OnDie -= UnBorrow;
            champion.OnDie -= ResetTimer;
        }

        protected override void OnRemovedFeedbackEffects(Entity target)
        {
            if (Entity.isMaster) return;
            
            gsm.OnUpdateFeedback -= IncreaseTimeUnBorrowed;
            
            champion.OnAttackFeedback -= ResetTimer;
            champion.OnAttackFeedback -= UnBorrow;
            
            champion.OnDieFeedback -= UnBorrow;
            champion.OnDieFeedback -= ResetTimer;
        }
    }
}


