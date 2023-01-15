using System;
using Entities;
using Entities.Capacities;
using Entities.Champion;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UIComponents
{
    public class UIPassiveIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Components")] [SerializeField]
        private Image backGroundImage;

        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI stackCount;
        [SerializeField] private Image overlayImage;

        [Header("Config")] [SerializeField] private Color positiveColor;
        [SerializeField] private Color negativeColor;

        private PassiveCapacity linkedCapacity;
        private Champion linkedChampion;

        private void Start()
        {
            overlayImage.fillAmount = 0;
        }

        private void Update()
        {
            if (!(linkedCapacity is {isOnCooldown: true})) return;

            overlayImage.fillAmount = (float) linkedCapacity.internalPassiveTimer / (float) linkedCapacity.duration;
        }

        public void LinkWithChampion(Champion champion, PassiveCapacity capacity, Action<Entity> removedCallback)
        {
            linkedChampion = champion;
            linkedCapacity = capacity;

            UpdatePassive(linkedChampion);

            capacity.OnAddedEffectsFeedbackCallback += UpdatePassive;

            capacity.OnRemovedEffectsFeedbackCallback += removedCallback;
        }

        public bool RemovePassive(Entity _)
        {
            if (!linkedCapacity.stackable) return true;

            if (linkedCapacity.count <= 0) return true;

            UpdatePassive(_);

            return false;
        }

        private void UpdatePassive(Entity _)
        {
            backGroundImage.color =
                linkedCapacity.AssociatedPassiveCapacitySO().types.Contains(Enums.CapacityType.Positive)
                    ? positiveColor
                    : negativeColor;
            var iconSprite = linkedCapacity.AssociatedPassiveCapacitySO().icon;

            if (iconSprite != null) icon.sprite = iconSprite;
            icon.color = iconSprite != null ? Color.white : TransparentColor();

            stackCount.text = linkedCapacity.stackable ? $"{linkedCapacity.count}" : "";
        }

        private Color TransparentColor()
        {
            var color = Color.white;
            color.a = 0;
            return color;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            ToolTipManager.Show(linkedCapacity.AssociatedPassiveCapacitySO().description,linkedCapacity.AssociatedPassiveCapacitySO().passiveName);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ToolTipManager.Hide();
        }
    }
}