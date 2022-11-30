using UnityEngine;
using UnityEngine.UI;

namespace UIComponents.Lobby
{
    public class RoleButton : MonoBehaviour
    {
        [SerializeField] private Image roleImage;
        [SerializeField] private Button button;
        private Enums.ChampionRole assignedRole;
        private LobbyUI assignedLobby;

        public void InitButton(Enums.ChampionRole role, Sprite sprite,LobbyUI lobby)
        {
            assignedLobby = lobby;
            
            roleImage.sprite = sprite;
            assignedRole = role;
            
            button.onClick.AddListener(OnButtonClick);
        }
    
        private void OnButtonClick()
        {
            assignedLobby.SetRole(assignedRole);
        }
    }
}


