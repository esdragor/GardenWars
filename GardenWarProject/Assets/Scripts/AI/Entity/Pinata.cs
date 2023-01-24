using System;
using DG.Tweening;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace Entities
{
    using FogOfWar;
    using Inventory;

    public partial class Pinata : Entity, IDeadable
    {
        [Header("Visual")]
        [SerializeField] private Transform FalseFX;
        [SerializeField] private GameObject Mesh;
        [SerializeField] private GameObject PinataDieFX;
        [SerializeField] private ParticleSystem HitFX;
        [SerializeField] private Animator[] Myanimators;
        [SerializeField] private float deZoomFov = 65f;
        [SerializeField] private float deZoomDuration = 1f;

        [Header("Bonbon Gaming")]
        [SerializeField] private CandyFollow candyFollow;
        private GameObject candyFollowGo;
         public float fillRange = 2;
        public static int level;
        private int maxBonbon => 30 + 10 * (level-1 + (level-1)%2);
        [SerializeField] private int currentBonbon;
        [SerializeField] private float timeBeforeDrain = 0.5f;
        [SerializeField] private Vector3 maxSize;
        [SerializeField] private ProjectileOnCollideEffect upgradeProjectile;
        
        private bool playerIsFigher;
        private Champion.Champion currentFeeder;
        
        
        [Header("Indicator")]
        [SerializeField] private RectTransform indicator;
        [SerializeField] private Image indicatorImage;
        [SerializeField] private float offset;
        [SerializeField] private float margin;
        
        //private GameObject rotatorGo;
        private Vector3 iconPos;
        private Camera cam;
        
        private bool isAlive = true;
        private bool canDie = true;

        public string SFXPinataDie;
        
        protected override void OnStart()
        {
            OnInstantiated();
            animators = Myanimators;
            
            playerIsFigher = gsm.GetPlayerChampion().isFighter;
            
            indicator.gameObject.SetActive(false);

            candyFollowGo = candyFollow.gameObject;
            candyFollowGo.SetActive(false);

            SetupLine();
        }

        public override void OnInstantiated()
        {
            isAlive = true;
            canDie = true;
            
            cam = Camera.main;
            
            currentBonbon = 0;

            indicatorImage.fillAmount = 0;

            currentFeeder = null;
            
            transform.localScale = Vector3.one;

            //UIManager.Instance.InstantiateHealthBarForEntity(this);
        }

        protected override void OnUpdate()
        {
            if(playerIsFigher) return;
            ShowIcon();
            UpdatePath();
        }

        private void ShowIcon()
        {
            iconPos = cam.WorldToScreenPoint(position+Vector3.up * offset);
            
            var sizeDelta = indicator.sizeDelta;
            var totalSize = sizeDelta * 0.5f + Vector2.one * margin;
            
            indicator.position = iconPos;

            isOnScreen = !((iconPos.x + totalSize.x > Screen.width) || (iconPos.x - totalSize.x < 0) ||
                           (iconPos.y + totalSize.y > Screen.height) || (iconPos.y - totalSize.y < 0));
            
            indicator.gameObject.SetActive(isOnScreen);
            
            /*
            if (iconPos.x + totalSize.x > Screen.width ) iconPos.x = Screen.width - totalSize.x;
            if (iconPos.x - totalSize.x < 0) iconPos.x = 0 + totalSize.x;
            if (iconPos.y + totalSize.y > Screen.height) iconPos.y = Screen.height - totalSize.y;
            if (iconPos.y - totalSize.y < 0) iconPos.y = 0 + totalSize.y;

            rotatorGo.SetActive(entityPos.x + margin > Screen.width || entityPos.y + margin > Screen.height || entityPos.x - margin < 0 || entityPos.y - margin < 0);
            
            if(actualPos == iconPos) return;
            
            var targetDir = cam.WorldToScreenPoint(transform.position) - rotator.position;
            rotator.up = targetDir;
            */
        }

        public bool IsAlive()
        {
            return isAlive;
        }

        public bool CanDie()
        {
            return canDie;
        }

        public void RequestSetCanDie(bool value)
        {
            photonView.RPC("SetCanDieRPC", RpcTarget.MasterClient, value);
        }

        public void SyncSetCanDieRPC(bool value)
        {
            canDie = value;
        }

        public void SetCanDieRPC(bool value)
        {
            canDie = value;
            photonView.RPC("SyncSetCanDieRPC", RpcTarget.All, value);
        }

        public event GlobalDelegates.BoolDelegate OnSetCanDie;
        public event GlobalDelegates.BoolDelegate OnSetCanDieFeedback;

        public void RequestDie(int KillerID)
        {
            photonView.RPC("DieRPC", RpcTarget.MasterClient, KillerID);
        }

        [PunRPC]
        public void DieRPC(int killerId)
        {
            OnDie?.Invoke(killerId);

            if (isOffline)
            {
                SyncDieRPC(killerId);

                return;
            }

            photonView.RPC("SyncDieRPC", RpcTarget.All, killerId);
        }

        [PunRPC]
        public void SyncDieRPC(int killerId)
        {
            isAlive = false;
            FogOfWarManager.Instance.RemoveFOWViewable(this);

            Destroy(LocalPoolManager.PoolInstantiate(PinataDieFX, transform.position, Quaternion.identity), 2f);

            FMODUnity.RuntimeManager.PlayOneShot("event:/" + SFXPinataDie, transform.position);
            
            DropUpgrade();
            
            SetAnimatorTrigger("Death");

            gameObject.SetActive(false);

            OnDieFeedback?.Invoke(killerId);
        }

        public event Action<int> OnDie;
        public event Action<int> OnDieFeedback;

        public void RequestRevive()
        {
        }

        public void SyncReviveRPC()
        {
        }

        public void ReviveRPC()
        {
        }

        public event GlobalDelegates.NoParameterDelegate OnRevive;
        public event GlobalDelegates.NoParameterDelegate OnReviveFeedback;
        
        public void StartChanneling(Champion.Champion champion)
        {
            if (currentFeeder != null)
            {
                return;
            }
            
            currentFeeder = champion;
            
            photonView.RPC("SyncZoomRPC",RpcTarget.All,currentFeeder.entityIndex,true);

            var pos = champion.position;
            pos.y = transform.position.y;
            transform.LookAt(pos);
            
            double drainTimer = -timeBeforeDrain;
            var drainTime = 1;

            gsm.OnTick += ChannelDrain;
            
            currentFeeder.OnDecreaseCurrentHp += CancelOnDamage;
            currentFeeder.OnMoving += CancelOnMove;
            currentFeeder.OnCast += CancelOnCast;
            currentFeeder.OnAttack += CancelOnCast;
            currentFeeder.OnDie += CancelOnDie;
            
            void ChannelDrain()
            {
                drainTimer += gsm.increasePerTick;
                
                if(drainTimer < 1) return;
                
                drainTimer = 0;
                drainTime++;

                if (champion.currentCandy <= 0)
                {
                    StopChanneling();
                    return;
                }
                
                DrainCandy(drainTime);
            }
            
            void DrainCandy(int candy)
            {
                if (currentFeeder.currentCandy - candy <= 0) candy = currentFeeder.currentCandy;

                photonView.RPC("SyncShootCandyRPC",RpcTarget.All,currentFeeder.entityIndex);

                IncreaseCurrentCandy(candy);
            }

            void IncreaseCurrentCandy(int candy)
            {
                currentFeeder.DecreaseCurrentCandyRPC(candy);
                
                currentBonbon += candy;

                transform.localScale = new Vector3(Mathf.Lerp(1, maxSize.x, (float) currentBonbon / maxBonbon), 1,
                    Mathf.Lerp(1, maxSize.z, (float) currentBonbon / maxBonbon));

                photonView.RPC("SyncImageFillRPC",RpcTarget.All,(float)currentBonbon/maxBonbon);

                if (currentBonbon < maxBonbon) return;
                
                DieRPC(currentFeeder.entityIndex);
                
                StopChanneling();
            }

            void CancelOnDie(int _)
            {
                StopChanneling();
            }
            
            void CancelOnCast(byte _,int __, Vector3 ___)
            {
                StopChanneling();
            }

            void CancelOnDamage(float _,int __)
            {
                StopChanneling();
            }
        
            void CancelOnMove(bool moving)
            {
                if(moving) StopChanneling();
            }
        
            void StopChanneling()
            {
                photonView.RPC("SyncZoomRPC",RpcTarget.All,currentFeeder.entityIndex,false);
                
                candyFollowGo.SetActive(false);
                
                gsm.OnTick -= ChannelDrain;

                currentFeeder.OnDecreaseCurrentHp -= CancelOnDamage;
                currentFeeder.OnMoving -= CancelOnMove;
                currentFeeder.OnCast -= CancelOnCast;
                currentFeeder.OnAttack -= CancelOnCast;
                currentFeeder.OnDie -= CancelOnDie;
                
                currentFeeder = null;
            }
        }
        
        [PunRPC]
        private void SyncZoomRPC(int championIndex,bool value)
        {
            if (championIndex != gsm.GetPlayerChampion().entityIndex) return;

            var zoomFov = value ? deZoomFov : 60;
            
            cam.DOFieldOfView(zoomFov,deZoomDuration);
        }
        
        [PunRPC]
        private void SyncShootCandyRPC(int championIndex)
        {
            if (championIndex != gsm.GetPlayerChampion().entityIndex) return;
            
            candyFollowGo.SetActive(true);
            candyFollow.transform.position = currentFeeder.position;
            candyFollow.SetTarget(this,2,0,false);
        }

        [PunRPC]
        private void SyncImageFillRPC(float value)
        {
            indicatorImage.fillAmount = value;
        }

        private void DropUpgrade()
        {
            var projectile = LocalPoolManager.PoolInstantiate(upgradeProjectile, transform.position+Vector3.up*0.5f, Quaternion.identity);

            projectile.OnEntityCollideFeedback += GiveUpgrade;
            
            void GiveUpgrade(Entity entity)
            {
                if (!(entity is Champion.Champion champion)) return;
                
                if(champion.upgrades >= 1) return;
                
                if(isMaster)champion.IncreaseUpgradeCount();
                
                projectile.DestroyProjectile(true);
            }
        }

    }
}