using Entities;
using UnityEngine;

namespace Controllers.Inputs
{
    public abstract class PlayerInputController : Controller
    {
        protected PlayerInputs inputs;

        protected RectTransform minimapRect;

        protected LayerMask layersToHit;
        protected Camera mainCam;


        protected Vector3 cursorWorldPos;
        protected Entity selectedEntities;


        protected override void OnAwake()
        {
            layersToHit = 1 << 9 | 1 << 29;
        }

        /// <summary>
        /// Setup the InputMap of The Player inputs
        /// </summary>
        private void SetupInputMap()
        {
            InputManager.PlayerMap = new PlayerInputs();
            InputManager.PlayerMap.Enable();
            inputs = InputManager.PlayerMap;
        }

        public void LinkControlsToPlayer()
        {
            if (!controlledEntity.photonView.IsMine) return;
            SetupInputMap();
            minimapRect = UIManager.Instance.GetMinimapRect();
            Debug.Log($"MinimapRect is {minimapRect}");
            mainCam = Camera.main;
            Link(controlledEntity);
        }

        public void LinkCameraToPlayer()
        {
            if (!controlledEntity.photonView.IsMine) return;
            CameraController.Instance.LinkCamera(controlledEntity.transform);
        }

        protected void UpdateTargets()
        {
            CastCamRay(out var hit);
            cursorWorldPos = hit.point;
            selectedEntities = null;
            if (hit.transform == null)
            {
                Debug.Log($"Wesh {hit.collider}");
                return;
            }
            var ent = hit.transform.GetComponent<Entity>();
            if (ent == null && hit.transform.parent != null) hit.transform.parent.GetComponent<Entity>();
            if (ent == null) return;
            selectedEntities = ent;
            cursorWorldPos = ent.transform.position;
        }

        private void CastCamRay(out RaycastHit hit)
        {
            var mousePos = Input.mousePosition;
            var mouseRay = RectTransformUtility.RectangleContainsScreenPoint(minimapRect, mousePos)
                ? UIManager.Instance.MiniMapCamRay(mousePos) : mainCam.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(mouseRay, out hit, Mathf.Infinity, layersToHit);
        }
    }
}