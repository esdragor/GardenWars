using System;
using Controllers.Inputs;
using Entities.Capacities;
using Entities.Champion;
using Entities.FogOfWar;
using Entities.Inventory;
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
            var soIndex = Array.IndexOf(GameStateMachine.Instance.allChampionsSo, championSO);
            
            GameStateMachine.AddOfflinePlayer(champion,team,role);

            controller.LinkControlsToPlayer();
            controller.LinkCameraToPlayer();
            
            //LoadMap
            CapacitySOCollectionManager.Instance.SetIndexes();
            
            UIManager.Instance.SetupEmoteWheel();
            
            UIManager.Instance.HideSettings();
            
            foreach (var championSo in GameStateMachine.Instance.allChampionsSo)
            {
                championSo.SetIndexes();
            }
            
            ItemCollectionManager.Instance.LinkCapacityIndexes();
            
            LocalPoolManager.Init();
            
            NetworkPoolManager.Init();
            
            //Late Load
            champion.ApplyChampionSO((byte)soIndex, team,role);
            
            UIManager.Instance.InstantiateChampionHUD();
            
            //UIManager.Instance.SetupTopBar();

            UIManager.Instance.AssignInventory(-1);
            
            GameStateMachine.SetupChampion(champion);

            if(minion != null) minion.InitEntity(Enums.Team.Neutral);
            
            FogOfWarManager.RunFog();
        }
        
        
        
        
    }
}

