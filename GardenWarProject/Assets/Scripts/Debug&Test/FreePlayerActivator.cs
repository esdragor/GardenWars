using Controllers.Inputs;
using Entities.Champion;
using GameStates;
using UnityEngine;

namespace FreePlayer
{
    public class FreePlayerActivator : MonoBehaviour
    {
        [SerializeField] private Champion champion;
        [SerializeField] private Enums.Team team;
        [SerializeField] private Enums.ChampionRole role;
        [SerializeField] private Minion minion;

        private void Start()
        {
            var controller = champion.GetComponent<PlayerInputController>();
            controller.LinkControlsToPlayer();
            controller.LinkCameraToPlayer();
            
            foreach (var championSo in GameStateMachine.Instance.allChampionsSo)
            {
                championSo.SetIndexes();
            }
            
            champion.ApplyChampionSO(1, Enums.Team.Team1,Enums.ChampionRole.Scavenger);
            
            GameStateMachine.AddOfflinePlayer(champion,team,role);

            GameStateMachine.SetupChampion(champion);
            
            UIManager.Instance.AssignInventory(-1);
            
            if(minion != null) minion.InitEntity(Enums.Team.Neutral);
        }
        
        
        
        
    }
}

