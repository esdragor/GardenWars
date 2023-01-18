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
        private string eventNameSelectionChampion;

        public void InitButton(byte index,LobbyUI lobby)
        {
            assignedLobby = lobby;
            gameStateMachine = GameStateMachine.Instance;
            
            championPortraitImage.sprite = gameStateMachine.allChampionsSo[index].SpriteSelection;
            championSOIndex = index;
            eventNameSelectionChampion = gameStateMachine.allChampionsSo[index].EventNameSelectionChampion;
            
            button.onClick.AddListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            assignedLobby.SetChampion(championSOIndex);
            assignedLobby.SetRole((Enums.ChampionRole)championSOIndex);
            FMODUnity.RuntimeManager.PlayOneShot("event:/" + eventNameSelectionChampion);

        }
    }
}