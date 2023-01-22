using System.Collections.Generic;
using System.Linq;
using Entities;
using Entities.Capacities;
using Entities.Champion;
using GameStates;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UIComponents
{
    public class UIChampionHUD : MonoBehaviour
    {
        [Header("Passive")]
        [SerializeField] private UIPassiveIcon passiveIconPrefab;
        [SerializeField] private Transform positivePassiveParent;
        [SerializeField] private Transform negativePassiveParent;
        [SerializeField] private UIPassiveIcon championPassiveIcon;

        [Header("Active")]
        [SerializeField] private UIActiveIcon activeIconPrefab;
        [SerializeField] private Transform activeIconParent;
        
        
        [Header("Components")]
        [SerializeField] private Image Portrait;
        [SerializeField] private Image healthBar;
        [SerializeField] private TMP_Text healthBarText;
        [SerializeField] private Image resourceBar;
        [SerializeField] private Image spellPassive;
        [SerializeField] private Image spellPassiveCooldown;

        [Header("Candy")]
        [SerializeField] private TMP_Text nbCandy;
        
        private Champion champion;
        private IResourceable resourceable;
        private IActiveLifeable lifeable;
        private ICastable castable;
        private ICandyable candyable;

        private Dictionary<PassiveCapacity, UIPassiveIcon> handledPassives = new Dictionary<PassiveCapacity, UIPassiveIcon>();

        private UIActiveIcon[] activeIcons = new UIActiveIcon[3];
        
        public void InitHUD(Champion newChampion)
        {
            champion = newChampion;
            lifeable = champion.GetComponent<IActiveLifeable>();
            resourceable = champion.GetComponent<IResourceable>();
            castable = champion.GetComponent<ICastable>();
            candyable = champion.GetComponent<ICandyable>();

            healthBar.fillAmount = lifeable.GetCurrentHpPercent();
            resourceBar.fillAmount = resourceable.GetCurrentResourcePercent();
            healthBarText.text = $"{lifeable.GetCurrentHp()}/{lifeable.GetMaxHp()}";
            
            handledPassives.Clear();

            for (int i = 0; i < 3; i++)
            {
                activeIcons[i] = Instantiate(activeIconPrefab, activeIconParent);
            }
            
            LinkToEvents();
            
            UpdateIcons(champion);
            
            UpdateCandy(0);
        }
        
        private void UpdateCandy(int _)
        {
            nbCandy.text = candyable.GetCurrentCandy().ToString();
        }

        private void LinkToEvents()
        {
            lifeable.OnSetCurrentHpFeedback += UpdateFillPercentHealth;
            lifeable.OnSetCurrentHpPercentFeedback += UpdateFillPercentByPercentHealth;
            lifeable.OnIncreaseCurrentHpFeedback += UpdateFillPercentHealth;
            lifeable.OnDecreaseCurrentHpFeedback += UpdateFillPercentHealth;
            lifeable.OnIncreaseMaxHpFeedback += UpdateFillPercentHealth;
            lifeable.OnDecreaseMaxHpFeedback += UpdateFillPercentHealth;

            resourceable.OnSetCurrentResourceFeedback += UpdateFillPercentResource;
            resourceable.OnSetCurrentResourcePercentFeedback += UpdateFillPercentByPercentResource;
            resourceable.OnIncreaseCurrentResourceFeedback += UpdateFillPercentResource;
            resourceable.OnDecreaseCurrentResourceFeedback += UpdateFillPercentResource;
            resourceable.OnIncreaseMaxResourceFeedback += UpdateFillPercentResource;
            resourceable.OnDecreaseMaxResourceFeedback += UpdateFillPercentResource;
            
            candyable.OnDecreaseCurrentCandyFeedback += UpdateCandy;
            candyable.OnIncreaseCurrentCandyFeedback += UpdateCandy;

            champion.OnPassiveCapacityAddedFeedback += AddPassiveIcon;

            champion.OnChangedAbilityFeedback += ChangeAbilityIcon;
        }

        private void ChangeAbilityIcon(int index,ActiveCapacity capacity)
        {
            activeIcons[index].SetCapacity(capacity,new InputAction());
        }

        private void UpdateIcons(Champion champion)
        {
            var so = champion.currentSo;
            
            Portrait.sprite = so.portrait;

            foreach (var passiveCapacity in champion.passiveCapacitiesList)
            {
                AddPassiveIcon(passiveCapacity);
            }

            for (var i = 0; i < champion.abilitiesIndexes.Length; i++)
            {
                var index = champion.abilitiesIndexes[i];
                ChangeAbilityIcon(i,champion.capacityDict[index].capacity);
            }
        }

        private void UpdateFillPercentByPercentHealth(float value)
        {
            healthBar.fillAmount = lifeable.GetCurrentHp() / lifeable.GetMaxHp();
            healthBarText.text = $"{lifeable.GetCurrentHp()}/{lifeable.GetMaxHp()}";
        }

        private void UpdateFillPercentHealth(float value)
        {
            healthBar.fillAmount = lifeable.GetCurrentHp() / lifeable.GetMaxHp();
            healthBarText.text = $"{lifeable.GetCurrentHp()}/{lifeable.GetMaxHp()}";

        }
        
        private void UpdateFillPercentHealth(float value,int _)
        {
            healthBar.fillAmount = lifeable.GetCurrentHp() / lifeable.GetMaxHp();
            healthBarText.text = $"{lifeable.GetCurrentHp()}/{lifeable.GetMaxHp()}";

        }

        private void UpdateFillPercentByPercentResource(float value)
        {
            resourceBar.fillAmount = value;
        }

        private void UpdateFillPercentResource(float value)
        {
            resourceBar.fillAmount = resourceable.GetCurrentResource();
        }

        private void AddPassiveIcon(PassiveCapacity capacity)
        {
            if (handledPassives.ContainsKey(capacity)) return;
            
            if (capacity.AssociatedPassiveCapacitySO().types.Contains(Enums.CapacityType.Kit))
            {
                handledPassives.Add(capacity,championPassiveIcon);
                championPassiveIcon.LinkWithChampion(champion,capacity,null);
                return;
            }

            Transform parent = null;

            var types = capacity.AssociatedPassiveCapacitySO().types;
            
            if (types.Contains(Enums.CapacityType.Positive))
            {
                parent = positivePassiveParent;
            }
            if (types.Contains(Enums.CapacityType.Negative))
            {
                parent = negativePassiveParent;
            }
            
            if(parent == null) return;

            var passiveIcon = Instantiate(passiveIconPrefab, parent); // TODO - Pool

            handledPassives.Add(capacity,passiveIcon);
            
            passiveIcon.LinkWithChampion(champion,capacity,RemovePassiveIcon);
            
            void RemovePassiveIcon(Entity _)
            {
                if (passiveIcon == null) return;
                
                if (passiveIcon.RemovePassive(_))
                {
                    handledPassives.Remove(capacity);
                    Destroy(passiveIcon.gameObject);
                }

            }
        }

        
        
    }
}