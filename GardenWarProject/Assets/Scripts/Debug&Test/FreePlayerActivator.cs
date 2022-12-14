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
        [SerializeField] private Enums.Team team;
        [SerializeField] private Enums.ChampionRole role;
        [SerializeField] private Minion minion;

        private void Start()
        {
            var controller = champion.GetComponent<PlayerInputController>();
            controller.LinkControlsToPlayer();
            controller.LinkCameraToPlayer();
            
            CapacitySOCollectionManager.Instance.SetIndexes();
            
            foreach (var championSo in GameStateMachine.Instance.allChampionsSo)
            {
                championSo.SetIndexes();
            }
            
            champion.ApplyChampionSO(1, team,role);
            
            GameStateMachine.AddOfflinePlayer(champion,team,role);

            GameStateMachine.SetupChampion(champion);
            
            UIManager.Instance.AssignInventory(-1);
            
            if(minion != null) minion.InitEntity(Enums.Team.Neutral);
        }
        
        
        
        
    }
}

