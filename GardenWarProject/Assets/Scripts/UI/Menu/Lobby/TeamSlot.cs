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

        public void SetupSlot(Color color, int side, byte team)
        {
            assignedTeam = (Enums.Team)team;
            joinButton.onClick.RemoveAllListeners();
            side = side < 0 ? -1 : 1;
            transform.localScale = new Vector3(side, 1, 1);
            championNameTransform.localScale = new Vector3(side, 1, 1);
            championNameText.alignment = side == 1 ? TextAlignmentOptions.Left : TextAlignmentOptions.Right;
            playerNameTransform.localScale = new Vector3(side, 1, 1);
            playerNameTransform.localPosition = new Vector3(side*4, 1, 1);
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
            championImage.sprite = GameStateMachine.Instance.allChampionsSo[data.championSOIndex].portrait;
            championNameText.text = $"{GameStateMachine.Instance.allChampionsSo[data.championSOIndex]}";
            playerNameText.text = $"{data.name}";
            roleImage.sprite = data.role == Enums.ChampionRole.Fighter ? fighterSprite : scavengerSprite;

        }

        public Enums.Team GetTeam()
        {
            return assignedTeam;
        }
    }
}


