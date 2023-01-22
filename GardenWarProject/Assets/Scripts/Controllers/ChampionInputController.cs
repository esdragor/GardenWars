using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Entities;
using Entities.Capacities;
using Entities.Champion;
using UnityEngine.AI;

namespace Controllers.Inputs
{
    public class ChampionInputController : PlayerInputController
    {
        [Header("Feedbacks")]
        [SerializeField] private ParticleSystem clicFx;

        [SerializeField] private Color allyColor = Color.cyan;
        [SerializeField] private Color enemyColor = Color.yellow;
        
        private UIManager uim => UIManager.Instance;

        private Champion champion;

        private bool isRightClicking;

        private bool isUpgrading;

        private InputAction.CallbackContext nullCtx = new InputAction.CallbackContext();

        private Entity previousSelected;

        private void Update()
        {
            if (champion == null) return;
            UpdateTargets();
            champion.targetedEntities = (selectedEntity) ? selectedEntity.entityIndex : -1;
            champion.targetedPositions = cursorWorldPos;
            
            if(selectedEntity != null)
                if (champion.GetEnemyTeams().Contains(selectedEntity.team) && selectedEntity.isVisible)
                    champion.targetedPositions = selectedEntity.position;
            
            if (isRightClicking) OnRightClick(nullCtx);
        }

        /// <summary>
        /// Actions Performed on Attack Activation
        /// </summary>
        /// <param name="ctx"></param>
        private void OnAttack(InputAction.CallbackContext ctx)
        {
            champion.RequestAttack(champion.attackAbilityIndex, selectedEntity.entityIndex, cursorWorldPos);
        }

        /// <summary>
        /// Actions Performed on Show or Hide Shop
        /// </summary>
        /// <param name="ctx"></param>
        private void OnShowHideShop(InputAction.CallbackContext ctx)
        {
            UIManager.Instance.ShowHideShop();
        }

        private void OnPressCapacity0(InputAction.CallbackContext ctx)
        {
            if (isUpgrading) return;
            champion.RequestOnCastCapacity(champion.abilitiesIndexes[0]);
        }

        private void OnPressCapacity1(InputAction.CallbackContext ctx)
        {
            if (isUpgrading) return;
            champion.RequestOnCastCapacity(champion.abilitiesIndexes[1]);
        }

        private void OnPressCapacity2(InputAction.CallbackContext ctx)
        {
            if (isUpgrading) return;
            champion.RequestOnCastCapacity(champion.abilitiesIndexes[2]);
        }

        private void OnReleaseCapacity0(InputAction.CallbackContext ctx)
        {
            if (isUpgrading)
            {
                champion.RequestUpgrade(0);
                return;
            }

            champion.RequestOnReleaseCapacity(champion.abilitiesIndexes[0]);
        }

        private void OnReleaseCapacity1(InputAction.CallbackContext ctx)
        {
            if (isUpgrading)
            {
                champion.RequestUpgrade(1);
                return;
            }

            champion.RequestOnReleaseCapacity(champion.abilitiesIndexes[1]);
        }

        private void OnReleaseCapacity2(InputAction.CallbackContext ctx)
        {
            if (isUpgrading)
            {
                champion.RequestUpgrade(2);
                return;
            }

            champion.RequestOnReleaseCapacity(champion.abilitiesIndexes[2]);
        }


        private void OnPressThrowCapacity(InputAction.CallbackContext ctx)
        {
            champion.RequestOnCastCapacity(champion.throwAbilityIndex);
        }

        private void OnReleaseThrowCapacity(InputAction.CallbackContext ctx)
        {
            champion.RequestOnReleaseCapacity(champion.throwAbilityIndex);
        }
        
        
        private void OnPressRecallCapacity(InputAction.CallbackContext ctx)
        {
            champion.RequestOnCastCapacity(champion.recallAbilityIndex);
        }

        private void OnReleaseRecallCapacity(InputAction.CallbackContext ctx)
        {
            champion.RequestOnReleaseCapacity(champion.recallAbilityIndex);
        }
        
        private void OnPressConsumeCapacity(InputAction.CallbackContext ctx)
        {
            champion.RequestOnCastCapacity(champion.consumeAbilityIndex);
        }

        private void OnReleaseConsumeCapacity(InputAction.CallbackContext ctx)
        {
            champion.RequestOnReleaseCapacity(champion.consumeAbilityIndex);
        }


        private void OnPressItem0(InputAction.CallbackContext ctx)
        {
            champion.RequestPressItem(0, selectedEntity.entityIndex, cursorWorldPos);
        }

        private void OnPressItem1(InputAction.CallbackContext ctx)
        {
            champion.RequestPressItem(1, selectedEntity.entityIndex, cursorWorldPos);
        }

        private void OnPressItem2(InputAction.CallbackContext ctx)
        {
            champion.RequestPressItem(2, selectedEntity.entityIndex, cursorWorldPos);
        }

        private void OnReleaseItem0(InputAction.CallbackContext ctx)
        {
            champion.RequestReleaseItem(0, selectedEntity.entityIndex, cursorWorldPos);
        }

        private void OnReleaseItem1(InputAction.CallbackContext ctx)
        {
            champion.RequestReleaseItem(1, selectedEntity.entityIndex, cursorWorldPos);
        }

        private void OnReleaseItem2(InputAction.CallbackContext ctx)
        {
            champion.RequestReleaseItem(2, selectedEntity.entityIndex, cursorWorldPos);
        }

        private void OnMouseMove(InputAction.CallbackContext ctx)
        {
            mousePos = ctx.ReadValue<Vector2>();
            UpdateTargets();
        }

        private async void RequeueClick(ParticleSystem fx)
        {
            await Task.Delay(450);
            fx.gameObject.SetActive(false);
        }

        private void OnRightClick(InputAction.CallbackContext ctx)
        {
            champion.CancelMoveToTarget();
            if (!champion.isAlive) return;
            
            if (selectedEntity != null)
            {
                if (champion != selectedEntity && selectedEntity.team == champion.team)
                {
                    //StartMoveGetItem();
                    
                    champion.MoveToPosition(cursorWorldPos);
                    
                    if(previousSelected != null) previousSelected.Deselect();
                    selectedEntity.Select(allyColor);
                    previousSelected = selectedEntity;
                    return;
                }

                if (selectedEntity is Tower) return;

                if (selectedEntity is Pinata pinata)
                {
                    StartMovePinata(pinata);
                }
                else
                {
                    StartMoveAttack();
                }
                
                if(previousSelected != null) previousSelected.Deselect();
                
                if(champion != selectedEntity) selectedEntity.Select(enemyColor);
                
                previousSelected = selectedEntity;
                
                return;
            }

            champion.MoveToPosition(cursorWorldPos);
            
            if(previousSelected != null) previousSelected.Deselect();
            previousSelected = null;

            if (!isRightClicking)
            {
                RequeueClick(LocalPoolManager.PoolInstantiate(clicFx, ActiveCapacity.GetClosestValidPoint(cursorWorldPos), clicFx.transform.rotation));
            }
                
        }

        private void OnLeftClick(InputAction.CallbackContext ctx)
        {
            champion.CancelCast();
        }

        private void CancelMovement(InputAction.CallbackContext ctx)
        {
            if(previousSelected != null) previousSelected.Deselect();
            previousSelected = null;

            champion.CancelMoveToTarget();
            champion.CancelCast();
            champion.MoveToPosition(champion.position);
        }

        private void OnPressEmote(InputAction.CallbackContext ctx)
        {
            uim.ShowWheel(mousePos);
        }

        private void OnReleaseEmote(InputAction.CallbackContext ctx)
        {
            uim.HideWheel();
            champion.RequestPressEmote(uim.emoteIndex);
        }

        private void StartMoveAttack()
        {
            var entityToMoveTo = selectedEntity;

            champion.StartMoveToTarget(selectedEntity, champion.attackRange, RequestAttack);

            void RequestAttack()
            {
                champion.RequestAttack(champion.attackAbilityIndex, entityToMoveTo.entityIndex, cursorWorldPos);
            }
        }

        private void StartMovePinata(Pinata pinata)
        {
            if(champion.isFighter) return;
            
            champion.StartMoveToTarget(selectedEntity, pinata.fillRange, RequestPinata);

            void RequestPinata()
            {
                champion.RequestChannelPinata(pinata);
                champion.CancelMoveToTarget();
            }
        }

        private void StartMoveGetItem()
        {
            if (!selectedEntity) return;

            champion.StartMoveToTarget(selectedEntity, champion.recupItemRange, RequestGetItem);

            void RequestGetItem()
            {
                if (!selectedEntity) return;
                if (selectedEntity.items.Count > 0)
                    champion.RequestSwitchInventoryToInventory(selectedEntity.entityIndex, 0);
            }
        }

        private void OnPressRightClick(InputAction.CallbackContext ctx)
        {
            isRightClicking = true;
        }

        private void OnReleaseRightClick(InputAction.CallbackContext ctx)
        {
            isRightClicking = false;
        }

        private void OnPressUpgrade(InputAction.CallbackContext ctx)
        {
            isUpgrading = true;
        }

        private void OnReleaseUpgrade(InputAction.CallbackContext ctx)
        {
            isUpgrading = false;
        }
        
        private void OnPressShowRange(InputAction.CallbackContext ctx)
        {
            champion.ShowMaxRangeIndicator(champion.attackRange);
        }

        private void OnReleaseShowRange(InputAction.CallbackContext ctx)
        {
            champion.HideMaxRangeIndicator();
        }

        protected override void Link(Entity entity)
        {
            champion = controlledEntity as Champion;
            
            if(champion == null) return;
            
            base.Link(entity);
            
            champion.GetComponent<NavMeshAgent>().enabled = true;
            champion.rb.isKinematic = true;
            
            inputs.Inventory.ShowHideInventory.started += context => UIManager.Instance.ShowHideInventory(true);
            inputs.Inventory.ShowHideInventory.canceled += context => UIManager.Instance.ShowHideInventory(false);

            inputs.Attack.Attack.performed += OnAttack;

            inputs.Capacity.Capacity0.performed += OnPressCapacity0;
            inputs.Capacity.Capacity1.performed += OnPressCapacity1;
            inputs.Capacity.Capacity2.performed += OnPressCapacity2;
            inputs.Capacity.Capacity0.canceled += OnReleaseCapacity0;
            inputs.Capacity.Capacity1.canceled += OnReleaseCapacity1;
            inputs.Capacity.Capacity2.canceled += OnReleaseCapacity2;
            
            inputs.Capacity.RecalCapacity.performed += OnPressRecallCapacity;
            inputs.Capacity.RecalCapacity.canceled += OnReleaseRecallCapacity;
            
            inputs.Capacity.EatCapacity.performed += OnPressConsumeCapacity;
            inputs.Capacity.EatCapacity.canceled += OnReleaseConsumeCapacity;

            inputs.Capacity.ThrowCapacity.performed += OnPressThrowCapacity;
            inputs.Capacity.ThrowCapacity.canceled += OnReleaseThrowCapacity;
            
            inputs.Capacity.UpgradeCapacity.performed += OnPressUpgrade;
            inputs.Capacity.UpgradeCapacity.canceled += OnReleaseUpgrade;
            
            inputs.Capacity.ShowMaxRangeIndicator.performed += OnPressShowRange;
            inputs.Capacity.ShowMaxRangeIndicator.canceled += OnReleaseShowRange;

            inputs.Inventory.ShowHideShop.performed += OnShowHideShop;
            
            inputs.Inventory.ActivateItem0.performed += OnPressItem0;
            inputs.Inventory.ActivateItem1.performed += OnPressItem1;
            inputs.Inventory.ActivateItem2.performed += OnPressItem2;
            inputs.Inventory.ActivateItem0.canceled += OnReleaseItem0;
            inputs.Inventory.ActivateItem1.canceled += OnReleaseItem1;
            inputs.Inventory.ActivateItem2.canceled += OnReleaseItem2;
            
            inputs.MoveMouse.RightClick.performed += OnRightClick;

            inputs.MoveMouse.MousePos.performed += OnMouseMove;
            
            inputs.MoveMouse.LeftClick.performed += OnLeftClick;

            inputs.MoveMouse.HoldRightClick.performed += OnPressRightClick;
            inputs.MoveMouse.HoldRightClick.canceled += OnReleaseRightClick;
            
            inputs.Movement.CancelMove.canceled += CancelMovement;

            inputs.Emotes.EmoteWheel.performed += OnPressEmote;
            inputs.Emotes.EmoteWheel.canceled += OnReleaseEmote;
        }
        
        protected override void Unlink()
        {
            inputs.Attack.Attack.performed -= OnAttack;

            inputs.Capacity.Capacity0.performed -= OnPressCapacity0;
            inputs.Capacity.Capacity1.performed -= OnPressCapacity1;
            inputs.Capacity.Capacity2.performed -= OnPressCapacity2;
            inputs.Capacity.Capacity0.canceled -= OnReleaseCapacity0;
            inputs.Capacity.Capacity1.canceled -= OnReleaseCapacity1;
            inputs.Capacity.Capacity2.canceled -= OnReleaseCapacity2;
            
            inputs.Capacity.RecalCapacity.performed -= OnPressRecallCapacity;
            inputs.Capacity.RecalCapacity.canceled -= OnReleaseRecallCapacity;
            
            inputs.Capacity.EatCapacity.performed -= OnPressConsumeCapacity;
            inputs.Capacity.EatCapacity.canceled -= OnReleaseConsumeCapacity;

            inputs.Capacity.ThrowCapacity.performed -= OnPressThrowCapacity;
            inputs.Capacity.ThrowCapacity.canceled -= OnReleaseThrowCapacity;
            
            inputs.Capacity.UpgradeCapacity.performed -= OnPressUpgrade;
            inputs.Capacity.UpgradeCapacity.canceled -= OnReleaseUpgrade;
            
            inputs.Capacity.ShowMaxRangeIndicator.performed -= OnPressShowRange;
            inputs.Capacity.ShowMaxRangeIndicator.canceled -= OnReleaseShowRange;

            inputs.Inventory.ShowHideShop.performed -= OnShowHideShop;
            
            inputs.Inventory.ActivateItem0.performed -= OnPressItem0;
            inputs.Inventory.ActivateItem1.performed -= OnPressItem1;
            inputs.Inventory.ActivateItem2.performed -= OnPressItem2;
            inputs.Inventory.ActivateItem0.canceled -= OnReleaseItem0;
            inputs.Inventory.ActivateItem1.canceled -= OnReleaseItem1;
            inputs.Inventory.ActivateItem2.canceled -= OnReleaseItem2;
            
            inputs.MoveMouse.RightClick.performed -= OnRightClick;

            inputs.MoveMouse.MousePos.performed -= OnMouseMove;
            
            inputs.MoveMouse.LeftClick.performed -= OnLeftClick;

            inputs.MoveMouse.HoldRightClick.performed -= OnPressRightClick;
            inputs.MoveMouse.HoldRightClick.canceled -= OnReleaseRightClick;
            
            inputs.Movement.CancelMove.canceled -= CancelMovement;

            inputs.Emotes.EmoteWheel.performed -= OnPressEmote;
            inputs.Emotes.EmoteWheel.canceled -= OnReleaseEmote;

            CameraController.Instance.UnLinkCamera();

            isRightClicking = false;
        }
    }
}