using Entities.Capacities;
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
        

        private ActiveCapacity capacity;

        private InputAction inputAction;
        
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


        public void SetCapacity(ActiveCapacity active,InputBinding binding)
        {
            if (capacity != null)
            {
                capacity.OnCooldownEnded -= ResetFill;
            }

            capacity = active;
            iconImage.sprite = capacity.AssociatedActiveCapacitySO().icon;
            cooldownOverlayImage.fillAmount = 0;
            keyBindText.text = binding.name;

            capacity.OnCooldownEnded += ResetFill;
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