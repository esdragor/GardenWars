using UnityEngine;
using UnityEngine.InputSystem;
using Entities;
using Entities.Capacities;
using Entities.Champion;
using Entities.Inventory;
using Microsoft.Win32.SafeHandles;
using UnityEngine.AI;

namespace Controllers.Inputs
{
    public class ChampionInputController : PlayerInputController
    {
        private Champion champion;
        
        private Vector2 mousePos;
        private bool isRightClicking;
        private InputAction.CallbackContext nullCtx = new InputAction.CallbackContext();

        private void Update()
        {
            if (champion == null) return;
            UpdateTargets();
            champion.targetedEntities = (selectedEntity) ? selectedEntity.entityIndex : -1;
            champion.targetedPositions = cursorWorldPos;
            if (isRightClicking) OnMouseClick(nullCtx);
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
            champion.RequestOnCastCapacity(champion.abilitiesIndexes[0]);
        }

        private void OnPressCapacity1(InputAction.CallbackContext ctx)
        {
            champion.RequestOnCastCapacity(champion.abilitiesIndexes[1]);
        }

        private void OnPressCapacity2(InputAction.CallbackContext ctx)
        {
            champion.RequestOnCastCapacity(champion.abilitiesIndexes[2]);
        }

        private void OnReleaseCapacity0(InputAction.CallbackContext ctx)
        {
            champion.RequestOnReleaseCapacity(champion.abilitiesIndexes[0]);
        }

        private void OnReleaseCapacity1(InputAction.CallbackContext ctx)
        {
            champion.RequestOnReleaseCapacity(champion.abilitiesIndexes[1]);
        }

        private void OnReleaseCapacity2(InputAction.CallbackContext ctx)
        {
            champion.RequestOnReleaseCapacity(champion.abilitiesIndexes[2]);
        }


        private void OnPressThrowCapacity(InputAction.CallbackContext ctx)
        {
            champion.RequestOnCastCapacity(CapacitySOCollectionManager.GetThrowAbilityIndex(champion.role));
        }

        private void OnReleaseThrowCapacity(InputAction.CallbackContext ctx)
        {
            champion.RequestOnReleaseCapacity(CapacitySOCollectionManager.GetThrowAbilityIndex(champion.role));
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

        private void OnMouseClick(InputAction.CallbackContext ctx)
        {
            if (selectedEntity != null)
            {
                if (champion != selectedEntity && selectedEntity.team == champion.team)
                {
                    StartMoveGetItem();
                    return;
                }

                StartMoveAttack();
                return;
            }

            champion.CancelMoveToTarget();
            champion.MoveToPosition(cursorWorldPos);
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

        // ReSharper disable Unity.PerformanceAnalysis
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

        private void OnHoldRightClick(InputAction.CallbackContext ctx)
        {
            isRightClicking = true;
        }

        private void OnReleaseRightClick(InputAction.CallbackContext ctx)
        {
            isRightClicking = false;
        }

        protected override void Link(Entity entity)
        {
            champion = controlledEntity as Champion;
            base.Link(entity);

            inputs.Attack.Attack.performed += OnAttack;

            inputs.Capacity.Capacity0.performed += OnPressCapacity0;
            inputs.Capacity.Capacity1.performed += OnPressCapacity1;
            inputs.Capacity.Capacity2.performed += OnPressCapacity2;
            inputs.Capacity.Capacity0.canceled += OnReleaseCapacity0;
            inputs.Capacity.Capacity1.canceled += OnReleaseCapacity1;
            inputs.Capacity.Capacity2.canceled += OnReleaseCapacity2;

            inputs.Capacity.ThrowCapacity.performed += OnPressThrowCapacity;
            inputs.Capacity.ThrowCapacity.canceled += OnReleaseThrowCapacity;

            champion.GetComponent<NavMeshAgent>().enabled = true;
            inputs.MoveMouse.ActiveButton.performed += OnMouseClick;
            champion.rb.isKinematic = true;

            inputs.MoveMouse.MousePos.performed += OnMouseMove;

            inputs.Inventory.ActivateItem0.performed += OnPressItem0;
            inputs.Inventory.ActivateItem1.performed += OnPressItem1;
            inputs.Inventory.ActivateItem2.performed += OnPressItem2;
            inputs.Inventory.ActivateItem0.canceled += OnReleaseItem0;
            inputs.Inventory.ActivateItem1.canceled += OnReleaseItem1;
            inputs.Inventory.ActivateItem2.canceled += OnReleaseItem2;

            inputs.Inventory.ShowHideInventory.started += context => UIManager.Instance.ShowHideInventory(true);
            inputs.Inventory.ShowHideInventory.canceled += context => UIManager.Instance.ShowHideInventory(false);
            inputs.Inventory.ShowHideShop.performed += OnShowHideShop;
            inputs.MoveMouse.LeftClick.performed += DebugNavMeshPoint;

            inputs.MoveMouse.HoldRightClick.performed += OnHoldRightClick;
            inputs.MoveMouse.HoldRightClick.canceled += OnReleaseRightClick;
        }

        private void DebugNavMeshPoint(InputAction.CallbackContext ctx)
        {
            var point = ActiveCapacity.GetClosestValidPoint(cursorWorldPos);
            Debug.DrawLine(point, point + Vector3.up, Color.yellow, 1f);
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

            inputs.Capacity.ThrowCapacity.performed += OnPressThrowCapacity;
            inputs.Capacity.ThrowCapacity.canceled += OnReleaseThrowCapacity;

            inputs.Inventory.ShowHideShop.performed -= OnShowHideShop;

            inputs.MoveMouse.ActiveButton.performed -= OnMouseClick;

            inputs.MoveMouse.MousePos.performed -= OnMouseMove;

            inputs.Inventory.ActivateItem0.performed -= OnPressItem0;
            inputs.Inventory.ActivateItem1.performed -= OnPressItem1;
            inputs.Inventory.ActivateItem2.performed -= OnPressItem2;
            inputs.Inventory.ActivateItem0.canceled -= OnReleaseItem0;
            inputs.Inventory.ActivateItem1.canceled -= OnReleaseItem1;
            inputs.Inventory.ActivateItem2.canceled -= OnReleaseItem2;

            inputs.MoveMouse.HoldRightClick.performed -= OnHoldRightClick;

            CameraController.Instance.UnLinkCamera();

            isRightClicking = false;
        }
    }
}