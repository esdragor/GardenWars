using System;
using GameStates;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIComponents.Lobby
{
    public class TeamSlot : MonoBehaviour
    {
        [Header("GameObjects & Transforms")]
        [SerializeField] private GameObject joinButtonObj;
        [SerializeField] private GameObject showWhenSelectedObj;
        [SerializeField] private GameObject showWhenReady;
        [SerializeField] private RectTransform championNameTransform;
        [SerializeField] private RectTransform playerNameTransform;
        [SerializeField] private RectTransform roleImageTransform;
        [SerializeField] private RectTransform championImageTransform;

        [Header("Components")]
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Button joinButton;
        [SerializeField] private Image championImage;
        [SerializeField] private TextMeshProUGUI championNameText;
        [SerializeField] private TextMeshProUGUI playerNameText;
        [SerializeField] private Image roleImage;

        private GameStateMachine gameStateMachine;
        private Enums.Team assignedTeam;
        private LobbyUI assignedLobby;

        public void InitSlot(Color color, byte team,LobbyUI lobby)
        {
            assignedLobby = lobby;
            gameStateMachine = GameStateMachine.Instance;
            
            assignedTeam = (Enums.Team)team;
            
            joinButton.onClick.RemoveAllListeners();
            
            var side = team == 2 ? -1 : 1;
            joinButtonObj.transform.localScale = new Vector3(side, 1, 1);
            transform.localScale = new Vector3(side, 1, 1);
            roleImageTransform.localScale = new Vector3(side, 1, 1);
            championImageTransform.localScale = new Vector3(side, 1, 1);
            championNameTransform.localScale = new Vector3(side, 1, 1);
            championNameText.alignment = side == 1 ? TextAlignmentOptions.Left : TextAlignmentOptions.Right;
            playerNameTransform.localPosition = new Vector3(side*4, -25, 1);
            playerNameText.alignment = side == 1 ? TextAlignmentOptions.Left : TextAlignmentOptions.Right;
            
            backgroundImage.color = color;
            
            UpdateSlot(null,false);
            
            joinButton.onClick.AddListener(OnButtonClick);
        }

        public void UpdateSlot(GameStateMachine.PlayerData data,bool local)
        {
            joinButtonObj.SetActive(data == null);
            showWhenSelectedObj.SetActive(data != null);
            showWhenReady.SetActive(false);
            
            if (data == null) return;
            
            showWhenReady.SetActive(data.isReady);
            
            if (data.championSOIndex < gameStateMachine.allChampionsSo.Length)
            {
                var championSo = gameStateMachine.allChampionsSo[data.championSOIndex];
                if (championSo != null)
                {
                    championImage.sprite = championSo.portrait;
                    championNameText.text = $"{championSo.name}";
                }
            }
            else
            {
                championImage.sprite = null;
                championNameText.text = string.Empty;
            }
            playerNameText.text = $"{data.name}";
            playerNameText.color = local ? Color.yellow : Color.black;
            roleImage.sprite = gameStateMachine.roles[(byte)data.role].sprite;

        }

        public Enums.Team GetTeam()
        {
            return assignedTeam;
        }
        
        private void OnButtonClick()
        {
            assignedLobby.SetTeam(assignedTeam);
        }
    }
}


