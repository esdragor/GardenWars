using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controllers
{
    public class CameraController : MonoBehaviour
    {
        //Script for the camera to follow the player
        [SerializeField] private Transform player;
        [SerializeField] private Transform camParentTr;
        [SerializeField] private Camera cam;
        private Transform camTr;

        [SerializeField] private float cameraSpeed = 0.1f;

        private bool cameraLock = true;
        public static CameraController Instance;
        
        [SerializeField] private Vector3 offset;
        [SerializeField] private Vector3 rotation;
        [SerializeField] private float lerpSpeed;
        public void Awake()
        {
            if (Instance != null && Instance != this)
            {
                DestroyImmediate(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            if (Camera.allCamerasCount > 1) cam.GetComponent<AudioListener>().enabled = false;
            camTr = cam.transform;
        }

        public void LinkCamera(Transform target)
        {
            player = target;
            InputManager.PlayerMap.Camera.VerrouillerlaCamera.performed += OnToggleCameraLock;
        }

        public void UnLinkCamera()
        {
            player = null;
            InputManager.PlayerMap.Camera.VerrouillerlaCamera.performed -= OnToggleCameraLock;
        }

        /// <summary>
        /// Actions Performed on Toggle CameraLock
        /// </summary>
        /// <param name="ctx"></param>
        private void OnToggleCameraLock(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;
            cameraLock = !cameraLock;
        }

        private void LateUpdate()
        {
            UpdateCamera(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            UpdateCamera(Time.fixedTime);
        }

        private void UpdateCamera(float deltaTime)
        {
            //if the player is not null
            if (!player) return;
            Vector3 nextPos;

            //if the camera is locked the camera follows the player
            if (cameraLock)
            {
                camParentTr.position = player.position + offset;
            }
            else
            {
                nextPos = camParentTr.position;

                if (Input.mousePosition.x >= Screen.width - 1)
                {
                    nextPos += camParentTr.right * cameraSpeed;
                }

                if (Input.mousePosition.x <= 0)
                {
                    nextPos -= camParentTr.right * cameraSpeed;
                }

                if (Input.mousePosition.y >= Screen.height - 1)
                {
                    nextPos += camParentTr.forward * cameraSpeed;
                }

                if (Input.mousePosition.y <= 0)
                {
                    nextPos -= camParentTr.forward * cameraSpeed;
                }

                camParentTr.position = Vector3.Lerp(camTr.position, nextPos, deltaTime* lerpSpeed);
            }

            camTr.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
        }
    }
}