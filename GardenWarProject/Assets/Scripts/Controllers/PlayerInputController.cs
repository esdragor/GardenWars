using Entities;
using UnityEngine;

namespace Controllers.Inputs
{
    public abstract class PlayerInputController : Controller
    {
        protected PlayerInputs inputs;

        protected RectTransform minimapRect;

        protected LayerMask entityLayers;
        protected LayerMask worldLayers;
        protected Camera mainCam;
        
        protected Vector3 cursorWorldPos;
        public static Vector3 CursorWorldPos { get; protected set; }
        protected Entity selectedEntity;
        
        public static Vector2 mousePos { get; protected set; }

        protected override void OnAwake()
        {
            entityLayers = 1 << 29;
            worldLayers = 1 << 9;
        }

        /// <summary>
        /// Setup the InputMap of The Player inputs
        /// </summary>
        private void SetupInputMap()
        {
            InputManager.PlayerMap ??= new PlayerInputs();
            InputManager.PlayerMap.Enable();
            inputs = InputManager.PlayerMap;
        }

        public void LinkControlsToPlayer()
        {
            if (!controlledEntity.photonView.IsMine) return;
            SetupInputMap();
            minimapRect = UIManager.Instance.GetMinimapRect();
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
            CastCamRay(out var entityHit,out var worldHit);
            cursorWorldPos = worldHit.point;
            CursorWorldPos = cursorWorldPos;
            selectedEntity = null;
            if (entityHit.transform == null)
            {
                return;
            }
            var ent = entityHit.transform.GetComponent<Entity>();
            if (ent == null && entityHit.transform.parent != null) entityHit.transform.parent.GetComponent<Entity>();
            if (ent == null) return;
            selectedEntity = ent;
            //cursorWorldPos = ent.transform.position;
            //CursorWorldPos = cursorWorldPos;
        }

        private void CastCamRay(out RaycastHit entityHit,out RaycastHit worldHit)
        {
            var mousePos = Input.mousePosition;
            if(RectTransformUtility.RectangleContainsScreenPoint(minimapRect, mousePos)) Debug.Log("On Minimap");
            var mouseRay = RectTransformUtility.RectangleContainsScreenPoint(minimapRect, mousePos)
                ? UIManager.Instance.MiniMapCamRay(mousePos) : mainCam.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(mouseRay, out entityHit, Mathf.Infinity, entityLayers);
            Physics.Raycast(mouseRay, out worldHit, Mathf.Infinity, worldLayers);
        }
    }
}