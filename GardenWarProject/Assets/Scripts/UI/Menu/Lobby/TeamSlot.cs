using GameStates;
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
        [SerializeField] private RectTransform championNameTransform;
        [SerializeField] private RectTransform playerNameTransform;
        [SerializeField] private RectTransform roleImageTransform;
        [SerializeField] private RectTransform championImageTransform;
        [Header("Components")]
        [SerializeField] private Button joinButton;
        [SerializeField] private Image championImage;
        [SerializeField] private TextMeshProUGUI championNameText;
        [SerializeField] private TextMeshProUGUI playerNameText;
        [SerializeField] private Image roleImage;

        [Header("Placeholder")]
        [SerializeField] private Sprite fighterSprite;
        [SerializeField] private Sprite scavengerSprite;

        private Enums.Team assignedTeam;

        public void SetupSlot(Color color, byte team)
        {
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
            GetComponent<Image>().color = color;
            UpdateSlot(null);
            joinButton.onClick.AddListener(()=>{GameStateMachine.Instance.RequestSetTeam(team);});
        }

        public void UpdateSlot(GameStateMachine.PlayerData data)
        {
            joinButtonObj.SetActive(data == null);
            showWhenSelectedObj.SetActive(data != null);
            if (data == null) return;
            if (data.championSOIndex < GameStateMachine.Instance.allChampionsSo.Length)
            {
                var championSo = GameStateMachine.Instance.allChampionsSo[data.championSOIndex];
                if (championSo != null)
                {
                    championImage.sprite = championSo.portrait;
                    championNameText.text = $"{championSo}";
                }
            }
            else
            {
                championImage.sprite = null;
                championNameText.text = string.Empty;
            }
            playerNameText.text = $"{data.name}";
            roleImage.sprite = data.role == Enums.ChampionRole.Fighter ? fighterSprite : scavengerSprite;

        }

        public Enums.Team GetTeam()
        {
            return assignedTeam;
        }
    }
}


