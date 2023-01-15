using Entities.Capacities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UIComponents
{
    public class UIActiveIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private Image cooldownOverlayImage;
        [SerializeField] private TextMeshProUGUI cooldownOverLayText;

        private ActiveCapacity capacity;

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


        public void SetCapacity(ActiveCapacity active)
        {
            if (capacity != null)
            {
                capacity.OnCooldownEnded -= ResetFill;
            }

            capacity = active;
            iconImage.sprite = capacity.AssociatedActiveCapacitySO().icon;
            cooldownOverlayImage.fillAmount = 0;

            capacity.OnCooldownEnded += ResetFill;
        }

        private void ResetFill()
        {
            cooldownOverlayImage.fillAmount = 0;
            cooldownOverLayText.text = string.Empty;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            ToolTipManager.Show(capacity.AssociatedActiveCapacitySO().description,capacity.AssociatedActiveCapacitySO().capacityName);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ToolTipManager.Hide();
        }
    }
}