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
        private Champion champion;
        private int[] selectedEntity;
        private Vector3[] cursorWorldPos;
        private bool isMoving;
        private Vector2 mousePos;
        private Vector2 moveInput;
        private Vector3 moveVector;
        private Camera cam;

        private void Update()
        {
            if(champion == null) return;
            UpdateTargets();
            champion.targetedEntities = selectedEntity;
            champion.targetedPositions = cursorWorldPos;
        }

        /// <summary>
        /// Actions Performed on Attack Activation
        /// </summary>
        /// <param name="ctx"></param>
        private void OnAttack(InputAction.CallbackContext ctx)
        {
            champion.RequestAttack(champion.attackAbilityIndex,selectedEntity,cursorWorldPos);
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
        
        
        private void OnPressItem0(InputAction.CallbackContext ctx)
        {
            champion.RequestPressItem(0,selectedEntity,cursorWorldPos);
        }
        private void OnPressItem1(InputAction.CallbackContext ctx)
        {
            champion.RequestPressItem(1,selectedEntity,cursorWorldPos);
        }
        private void OnPressItem2(InputAction.CallbackContext ctx)
        {
            champion.RequestPressItem(2,selectedEntity,cursorWorldPos);
        }
        
        private void OnReleaseItem0(InputAction.CallbackContext ctx)
        {
            champion.RequestReleaseItem(0,selectedEntity,cursorWorldPos);
        }
        private void OnReleaseItem1(InputAction.CallbackContext ctx)
        {
            champion.RequestReleaseItem(1,selectedEntity,cursorWorldPos);
        }
        private void OnReleaseItem2(InputAction.CallbackContext ctx)
        {
            champion.RequestReleaseItem(2,selectedEntity,cursorWorldPos);
        }

        private void OnMouseMove(InputAction.CallbackContext ctx)
        {
            mousePos = ctx.ReadValue<Vector2>();
            UpdateTargets();
        }
        
        private void UpdateTargets()
        {
            var mouseRay = cam.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(mouseRay, out var hit, Mathf.Infinity, layersToHit)) return;
            cursorWorldPos[0] = hit.point;
            selectedEntity[0] = -1;
            var ent = hit.transform.GetComponent<Entity>();
            if (ent == null && hit.transform.parent != null) hit.transform.parent.GetComponent<Entity>();
            if (ent == null) return;
            selectedEntity[0] = ent.entityIndex;
            cursorWorldPos[0] = ent.transform.position;
        }
        
        private void OnMouseClick(InputAction.CallbackContext ctx)
        {
            if (selectedEntity[0] != -1)
            {
                champion.RequestAttack(champion.attackAbilityIndex, selectedEntity, cursorWorldPos);
                return;
            }
            champion.MoveToPosition(cursorWorldPos[0]);
        }

        protected override void Link(Entity entity)
        {
            champion = controlledEntity as Champion;
            base.Link(entity);
            
            cam = Camera.main;
            selectedEntity = new int[1];
            cursorWorldPos = new Vector3[1];
            
            inputs.Attack.Attack.performed += OnAttack;
            
            inputs.Capacity.Capacity0.performed += OnPressCapacity0;
            inputs.Capacity.Capacity1.performed += OnPressCapacity1;
            inputs.Capacity.Capacity2.performed += OnPressCapacity2;
            inputs.Capacity.Capacity0.canceled += OnReleaseCapacity0;
            inputs.Capacity.Capacity1.canceled += OnReleaseCapacity1;
            inputs.Capacity.Capacity2.canceled += OnReleaseCapacity2;
            
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

        }

        private void DebugNavMeshPoint(InputAction.CallbackContext ctx)
        {
            var point = ActiveCapacity.GetClosestValidPoint(cursorWorldPos[0]);
            Debug.DrawLine(point,point+Vector3.up,Color.yellow,1f);
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
            inputs.Inventory.ShowHideShop.performed -= OnShowHideShop;

            inputs.MoveMouse.ActiveButton.performed -= OnMouseClick;

            inputs.MoveMouse.MousePos.performed -= OnMouseMove;
            
            inputs.Inventory.ActivateItem0.performed -= OnPressItem0;
            inputs.Inventory.ActivateItem1.performed -= OnPressItem1;
            inputs.Inventory.ActivateItem2.performed -= OnPressItem2;
            inputs.Inventory.ActivateItem0.canceled -= OnReleaseItem0;
            inputs.Inventory.ActivateItem1.canceled -= OnReleaseItem1;
            inputs.Inventory.ActivateItem2.canceled -= OnReleaseItem2;

            CameraController.Instance.UnLinkCamera();
        }
    }
}
