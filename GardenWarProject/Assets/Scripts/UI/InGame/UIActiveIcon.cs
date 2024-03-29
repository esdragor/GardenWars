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
    public class UIActiveIcon : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private Image cooldownOverlayImage;
        [SerializeField] private TextMeshProUGUI cooldownOverLayText;
        [SerializeField] private TextMeshProUGUI keyBindText;
        [SerializeField] private Button upgradeButton;

        [SerializeField] private UIActiveIconDescription description;
        [SerializeField] private UIActiveIconUpgradeDescription upgradeDescription;
        private GameObject upgradeButtonGo;

        private int upgradeIndex;
        private ActiveCapacity capacity;
        private Champion champion;

        public InputControl control { get; private set; }

        private void Start()
        {
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
                capacity.OnUsable -= UpdateIconOpacity;
            }

            if(newControl != null) control = newControl;

            if(index != -1) upgradeIndex = index;
            capacity = active;
            
            champion = GameStateMachine.Instance.GetPlayerChampion();
            
            upgradeButtonGo = upgradeButton.gameObject;
            upgradeButtonGo.SetActive(false);

            if (champion.isFighter)
            {
                upgradeButton.onClick.AddListener(UpgradeAbility);

                champion.OnUpgradeCountIncreased += UpdateUpgradeActive;
                champion.OnUpgradeCountDecreased += UpdateUpgradeActive;
                capacity.OnUpgraded += HideUpgradeButton;
            }
            
            description.capacity = capacity;
            upgradeDescription.capacity = capacity;
            iconImage.sprite = capacity.AssociatedActiveCapacitySO().icon;
            cooldownOverlayImage.fillAmount = 0;
            var keyName = Keyboard.current.FindKeyOnCurrentKeyboardLayout(control.name).name;
            keyBindText.text = keyName.Length > 1 ? keyName : keyName.ToUpper();
            
            capacity.OnCooldownEnded += ResetFill;

            capacity.OnUsable += UpdateIconOpacity;
            
            capacity.Init();
        }

        private void UpdateIconOpacity(bool value)
        {
            iconImage.color = value ? Color.white : Color.grey;
        }

        private void UpdateUpgradeActive()
        {
            upgradeButtonGo.SetActive(champion.upgrades > 0 && capacity.canBeUpgraded);
        }
        
        private void HideUpgradeButton(int level)
        {
            description.DisplayText();
            upgradeDescription.DisplayText();
            upgradeButtonGo.SetActive(champion.upgrades > 0 && level < capacity.AssociatedActiveCapacitySO().maxLevel);
        }

        private void ResetFill()
        {
            cooldownOverlayImage.fillAmount = 0;
            cooldownOverLayText.text = string.Empty;
        }
    }
}