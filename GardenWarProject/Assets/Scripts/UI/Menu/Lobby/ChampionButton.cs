using GameStates;
using UnityEngine;
using UnityEngine.UI;

namespace UIComponents.Lobby
{
    public class ChampionButton : MonoBehaviour
    {
        [SerializeField] private Image championPortraitImage;
        [SerializeField] private Button button;
        private byte championSOIndex;
        private GameStateMachine gameStateMachine;
        private LobbyUI assignedLobby;

        public void InitButton(byte index,LobbyUI lobby)
        {
            assignedLobby = lobby;
            gameStateMachine = GameStateMachine.Instance;
            
            championPortraitImage.sprite = gameStateMachine.allChampionsSo[index].portrait;
            championSOIndex = index;
            
            button.onClick.AddListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            assignedLobby.SetChampion(championSOIndex);
        }
    }
}