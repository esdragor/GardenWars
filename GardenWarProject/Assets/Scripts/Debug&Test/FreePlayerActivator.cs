using Controllers.Inputs;
using Entities.Champion;
using GameStates;
using UnityEngine;

namespace FreePlayer
{
    public class FreePlayerActivator : MonoBehaviour
    {
        [SerializeField] private Champion champion;

        private void Start()
        {
            if(Camera.allCamerasCount > 1) Debug.LogWarning($"MORE THAN 1 CAMERA ON SCENE, ONLY USE CAMERA FROM CAMERA MANAGER");
            
            var controller = champion.GetComponent<PlayerInputController>();
            controller.LinkControlsToPlayer();
            controller.LinkCameraToPlayer();
            
            foreach (var championSo in GameStateMachine.Instance.allChampionsSo)
            {
                championSo.SetIndexes();
            }
            
            champion.ApplyChampionSO(1, Enums.Team.Team1);
            
            champion.SetupSpawn();
            champion.SetupNavMesh();
            champion.SetupUI();
        }
        
        
    }
}

