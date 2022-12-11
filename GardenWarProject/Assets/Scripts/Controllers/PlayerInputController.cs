using UnityEngine;

namespace Controllers.Inputs
{
    public abstract class PlayerInputController : Controller
    {
        protected PlayerInputs inputs;
        protected Vector3 cursorWorldPos;
        protected LayerMask layersToHit;

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
            if(!controlledEntity.photonView.IsMine) return;
            SetupInputMap();
            Link(controlledEntity);
        }

        public void LinkCameraToPlayer()
        {
            if(!controlledEntity.photonView.IsMine) return;
            CameraController.Instance.LinkCamera(controlledEntity.transform);
        }
    }
}
