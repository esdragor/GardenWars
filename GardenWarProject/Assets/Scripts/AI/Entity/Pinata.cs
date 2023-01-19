using System;
using Photon.Pun;
using UnityEngine;

namespace Entities
{
    using FogOfWar;
    using Inventory;

    public class Pinata : Entity, IDeadable
    {
        [Header("Visual")]
        [SerializeField] private Transform FalseFX;
        [SerializeField] private GameObject Mesh;
        [SerializeField] private GameObject PinataDieFX;
        [SerializeField] private ParticleSystem HitFX;
        [SerializeField] private Animator[] Myanimators;

        [Header("Bonbon Gaming")]
        public float fillRange = 4;
        public static int level;
        private int maxBonbon => level % 2 == 0 ? 30 + 20 * level : 30 + 20 * level - 1;
        [SerializeField] private int currentBonbon;

        [Header("Indicator")]
        [SerializeField] private RectTransform indicator;
        //[SerializeField] private RectTransform rotator;
        [SerializeField] private float offset;
        [SerializeField] private float margin;
        //private GameObject rotatorGo;
        private Vector3 iconPos;

        private bool playerIsFigher;
        
        private Camera cam;
        
        private bool isAlive = true;
        private bool canDie = true;


        protected override void OnStart()
        {
            OnInstantiated();
            animators = Myanimators;
            //rotatorGo = rotator.gameObject;
            //rotatorGo.SetActive(false);
            playerIsFigher = gsm.GetPlayerChampion().isFighter;
        }

        public override void OnInstantiated()
        {
            isAlive = true;
            
            cam = Camera.main;

            //UIManager.Instance.InstantiateHealthBarForEntity(this);
        }

        protected override void OnUpdate()
        {
            ShowIcon();
        }

        private void ShowIcon()
        {
            if(playerIsFigher) return;
            
            iconPos = cam.WorldToScreenPoint(position+Vector3.up * offset);
            
            var sizeDelta = indicator.sizeDelta;
            var totalSize = sizeDelta * 0.5f + Vector2.one * margin;
            
            indicator.position = iconPos;
            
            indicator.gameObject.SetActive(!((iconPos.x + totalSize.x > Screen.width) || (iconPos.x - totalSize.x < 0) ||
                                           (iconPos.y + totalSize.y > Screen.height) || (iconPos.y - totalSize.y < 0)));
            
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
            var entity = EntityCollectionManager.GetEntityByIndex(killerId);
            if (entity && (entity is Champion.Champion))
            {
                entity.AddItemRPC(items[0].indexOfSOInCollection);
            }

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

            Destroy(Instantiate(PinataDieFX, transform.position, Quaternion.identity), 2f);
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

        public void StartChanneling()
        {
            Debug.Log("YAYAYAYAYA");
        }
    }
}