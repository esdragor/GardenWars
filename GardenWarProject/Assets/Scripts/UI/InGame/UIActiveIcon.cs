using Entities.Capacities;
using Entities.Champion;
using GameStates;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UIComponents
{
    public class UIActiveIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private Image cooldownOverlayImage;
        [SerializeField] private TextMeshProUGUI cooldownOverLayText;
        [SerializeField] private TextMeshProUGUI keyBindText;
        [SerializeField] private Button upgradeButton;
        private GameObject upgradeButtonGo;

        private int upgradeIndex;
        private ActiveCapacity capacity;
        private Champion champion;

        public InputControl control { get; private set; }

        private void Start()
        {
            champion = GameStateMachine.Instance.GetPlayerChampion();
            
            upgradeButtonGo = upgradeButton.gameObject;
            //upgradeButtonGo.SetActive(false);
            
            upgradeButton.onClick.AddListener(UpgradeAbility);
            
            champion.OnUpgradeCountIncreased += UpdateUpgradeActive;
            champion.OnUpgradeCountDecreased += UpdateUpgradeActive;
            capacity.OnUpgraded += HideUpgradeButton;

            ResetFill();
        }

        private void Update()
        {
            if (!(capacity is {isOnCooldown: true})) return;

            cooldownOverlayImage.fillAmount = (float) capacity.cooldownTimer / (float) capacity.baseCooldown;

            cooldownOverLayText.text = $"{(int) capacity.cooldownTimer + 1}";
        }

        private void UpgradeAbility()
        {
            champion.RequestUpgrade(upgradeIndex);
        }


        public void SetCapacity(ActiveCapacity active,InputControl newControl,int index = -1)
        {
            if (capacity != null)
            {
                capacity.OnCooldownEnded -= ResetFill;
            }

            if(newControl != null) control = newControl;

            upgradeIndex = index;
            capacity = active;
            iconImage.sprite = capacity.AssociatedActiveCapacitySO().icon;
            cooldownOverlayImage.fillAmount = 0;
            keyBindText.text = control.name.Length > 1 ? control.name : control.name.ToUpper();

            capacity.OnCooldownEnded += ResetFill;
        }

        private void UpdateUpgradeActive()
        {
            upgradeButtonGo.SetActive(champion.upgrades > 0 && capacity.canBeUpgraded);
        }
        
        private void HideUpgradeButton(int level)
        {
            
            upgradeButtonGo.SetActive(champion.upgrades > 0 && level < capacity.AssociatedActiveCapacitySO().maxLevel);
        }

        private void ResetFill()
        {
            cooldownOverlayImage.fillAmount = 0;
            cooldownOverLayText.text = string.Empty;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            var so = capacity.AssociatedActiveCapacitySO();

            var text = capacity.level switch
            {
                1 => so.description,
                2 => so.description1,
                3 => so.description2,
                _ => so.description
            };


            var level = GameStateMachine.Instance.GetPlayerChampion().isFighter ? $"[{capacity.level}]" : string.Empty;
            var header = $"{so.capacityName} {level}";
            
            ToolTipManager.Show(text,header);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ToolTipManager.Hide();
        }
    }
}