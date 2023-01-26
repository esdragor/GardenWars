using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Entities.Capacities
{
    [CreateAssetMenu(menuName = "Capacity/PassiveCapacitySO/SharkPassive", fileName = "SharkPassive")]
    public class SharkPassiveSO : PassiveCapacitySO
    {
        public double timeUntilBorrow = 3;
        public float borrowTransitionDuration = 1f;
        public float unBorrowTransitionDuration = 0.683f;
        public float borrowDamage = 10f;
        public float knockUpDamage = 20f;
        public float increasedMoveSpeed = 1f;
        public ProjectileOnCollideEffect aileron;
        public StunPassiveSO stunPassive;
        public ParticleSystem UnborrowBlue;
        public ParticleSystem UnborrowRed;
        
        public string SFXSharkPassive;

        public override Type AssociatedType()
        {
            return typeof(SharkPassive);
        }
    }

    public class SharkPassive : PassiveCapacity
    {
        public bool borrowed;
        public float bonusDamage = 0;
        private bool playedAnim;
        
        private SharkPassiveSO so => (SharkPassiveSO) AssociatedPassiveCapacitySO();

        private ProjectileOnCollideEffect aileron;
        private GameObject aileronGo;
        
        private double timeUnBorrowed;
        private ParticleSystem Unborrow;
        private GameObject UnborrowGO;

        public SharkUlt ult;

        protected override void OnAddedEffects(Entity target)
        {
            timeUnBorrowed = 0;
            borrowed = false;
            bonusDamage = 0;

            aileron = LocalPoolManager.PoolInstantiate(so.aileron, champion.rotateParent.position, champion.rotation,
                champion.rotateParent);
            aileronGo = aileron.gameObject;
            aileronGo.SetActive(false);

            aileron.OnEntityCollide += EntityCollide;
            
            gsm.OnUpdate += IncreaseTimeUnBorrowed;
            
            champion.OnAttack += ResetTimer;
            champion.OnAttack += UnBorrow;
            
            champion.OnDie += UnBorrow;
            champion.OnDie += ResetTimer;

            champion.OnStartChannelPinata += ResetTimer;
            champion.OnStartChannelPinata += ForceUnBorrow;
            
        }

        protected override void OnAddedFeedbackEffects(Entity target)
        {
            if (Entity.isMaster) return;

            aileron = LocalPoolManager.PoolInstantiate(so.aileron, champion.rotateParent.position, champion.rotation,
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

        protected override void OnAddedLocalEffects(Entity target)
        {
            
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

        public void Borrow(bool untargetable = false)
        {
            if(borrowed) return;
            
            FMODUnity.RuntimeManager.PlayOneShot("event:/" + so.SFXSharkPassive, champion.transform.position);
            champion.CurrentSFXMove.Stop();
            champion.CurrentSFXMove = champion.SFXMoves[1];
            champion.CurrentSFXMove.Play();           
            //champion.rotateParent.localPosition = Vector3.up * -0.75f;

            if (Entity.isMaster)
            {
                if(untargetable) champion.SetCanBeTargetedRPC(false);
                
                champion.IncreaseCurrentMoveSpeedRPC(so.increasedMoveSpeed);
                
                champion.OnAttack += StunTarget;
                
                OnBurrow?.Invoke();
            }

            OnBurrowFeedback?.Invoke();
            
            borrowed = true;
            
            champion.SetAnimatorBool("Borrowed",true);
            
            aileronGo.SetActive(true);
            aileron.OnEntityCollide += EntityCollide;

            ult?.UpdateUsable(true);

            aileronGo.GetComponent<SharkPassiveManager>().EnableFXShot(champion.team);
        }

        public event Action OnBurrow;
        public event Action OnBurrowFeedback;

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
            
            champion.CurrentSFXMove.Stop();
            champion.CurrentSFXMove = champion.SFXMoves[0];
            champion.CurrentSFXMove.Play();  

            ForceUnBorrow();
        }

        public void ForceUnBorrow()
        {
            if(!borrowed) return;
            
            champion.SetCanBeTargetedRPC(true);

            if (Entity.isMaster)
            {
                champion.OnAttack -= StunTarget;
                
                champion.DecreaseCurrentMoveSpeedRPC(so.increasedMoveSpeed);
                
                OnUnBurrow?.Invoke();
            }
            
            //champion.rotateParent.localPosition = Vector3.zero;

            borrowed = false;
            champion.SetAnimatorBool("Borrowed",false);
            
            ult?.UpdateUsable(false);
            
            if (!UnborrowGO)
            {
                Unborrow = champion.team == gsm.GetPlayerTeam() ? so.UnborrowBlue : so.UnborrowRed;
                UnborrowGO = LocalPoolManager.PoolInstantiate(Unborrow, champion.championMesh.transform).gameObject;
            }
            UnborrowGO.transform.localPosition = Vector3.zero;
            UnborrowGO.SetActive(false);
            UnborrowGO.SetActive(true);

            aileronGo.SetActive(false);
        }

        public event Action OnUnBurrow;
        
        private void UnBorrow(int _)
        {
            ForceUnBorrow();
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
        
        private void ResetTimer()
        {
            ResetTimer(0,0,Vector3.zero);
        }

        private void EntityCollide(Entity entity)
        {
            if (!champion.GetEnemyTeams().Contains(entity.team)) return;
            
            var lifeable = entity.GetComponent<IActiveLifeable>();
            
            lifeable?.DecreaseCurrentHpRPC(so.borrowDamage+bonusDamage, champion.entityIndex);
        }

        protected override void OnRemovedEffects(Entity target)
        {
            aileron.OnEntityCollide -= EntityCollide;
            
            gsm.OnUpdate -= IncreaseTimeUnBorrowed;
            
            champion.OnAttack -= ResetTimer;
            champion.OnAttack -= UnBorrow;
            
            champion.OnDie -= UnBorrow;
            champion.OnDie -= ResetTimer;
            
            champion.OnStartChannelPinata -= ResetTimer;
            champion.OnStartChannelPinata -= ForceUnBorrow;
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

        protected override void OnRemovedLocalEffects(Entity target)
        {
            
        }
    }
}


