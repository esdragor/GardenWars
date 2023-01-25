using System;
using DG.Tweening;
using GameStates;
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
        [SerializeField] private int maxCandyToFeed = 110;
        private GameObject candyFollowGo;
        public float fillRange = 2;
        public static int level;
        private int maxBonbon => bonbonFormule > maxCandyToFeed ? maxCandyToFeed : bonbonFormule;
        private int bonbonFormule => 30 + 10 * (level - 1 + (level - 1) % 2);
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
            
            cam = GameStateMachine.mainCam;
            
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
            
            if(killerId != entityIndex) DropUpgrade();
            
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
            var quarterSeconds = 0;
            var secondspassed = 0;
            
            bool sentMessage = false;

            gsm.OnTick += ChannelDrain;
            
            currentFeeder.OnDecreaseCurrentHp += CancelOnDamage;
            currentFeeder.OnMoving += CancelOnMove;
            currentFeeder.OnCast += CancelOnCast;
            currentFeeder.OnAttack += CancelOnCast;
            currentFeeder.OnDie += CancelOnDie;

            void ChannelDrain()
            {
                drainTimer += gsm.increasePerTick;
                
                if(drainTimer < 0.25) return;
                
                drainTimer = 0;

                if (!sentMessage)
                {
                    gsm.DisplayMessage(gsm.messageFeedingPinata);
                    sentMessage = true;
                }
                
                quarterSeconds++;
                if (quarterSeconds == 4)
                {
                    secondspassed++;
                    quarterSeconds = 0;
                }

                if (champion.currentCandy <= 0)
                {
                    StopChanneling();
                    return;
                }
                
                DrainCandy(secondspassed + 1);
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
                
                gsm.DisplayMessage(gsm.messageFedPinata);
                
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
            
            var fx = LocalPoolManager.PoolInstantiate(HitFX, currentFeeder.position+Vector3.up+currentFeeder.forward, Quaternion.Euler(-90,0,0)).gameObject;
            fx.SetActive(false);
            fx.SetActive(true);
            
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