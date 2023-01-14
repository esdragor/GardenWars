using System;
using Controllers.Inputs;
using Entities.Capacities;
using Entities.Champion;
using GameStates;
using UnityEngine;

namespace FreePlayer
{
    public class FreePlayerActivator : MonoBehaviour
    {
        [SerializeField] private Champion champion;
        [SerializeField] private ChampionSO championSO;
        [SerializeField] private Enums.Team team;
        [SerializeField] private Enums.ChampionRole role;
        [SerializeField] private Minion minion;

        private void Start()
        {
            var controller = champion.GetComponent<PlayerInputController>();
            controller.LinkControlsToPlayer();
            controller.LinkCameraToPlayer();
            
            CapacitySOCollectionManager.Instance.SetIndexes();
            
            LocalPoolManager.Init();
            
            NetworkPoolManager.Init();
            
            foreach (var championSo in GameStateMachine.Instance.allChampionsSo)
            {
                championSo.SetIndexes();
            }

            var soIndex = Array.IndexOf(GameStateMachine.Instance.allChampionsSo, championSO);
            
            champion.ApplyChampionSO((byte)soIndex, team,role);
            
            GameStateMachine.AddOfflinePlayer(champion,team,role);

            GameStateMachine.SetupChampion(champion);
            
            UIManager.Instance.InstantiateChampionHUD();
            
            UIManager.Instance.AssignInventory(-1);
            
            if(minion != null) minion.InitEntity(Enums.Team.Neutral);
        }
        
        
        
        
    }
}

