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

        [Header("Candy")]
        [SerializeField] private TMP_Text nbCandy;
        
        private Champion champion;
        private IResourceable resourceable;
        private IActiveLifeable lifeable;
        private ICandyable candyable;

        private Dictionary<PassiveCapacity, UIPassiveIcon> handledPassives = new Dictionary<PassiveCapacity, UIPassiveIcon>();
        
        private UIActiveIcon[] activeIcons = new UIActiveIcon[3];
        
        public void InitHUD(Champion newChampion)
        {
            champion = newChampion;
            lifeable = champion.GetComponent<IActiveLifeable>();
            resourceable = champion.GetComponent<IResourceable>();
            candyable = champion.GetComponent<ICandyable>();

            healthBar.fillAmount = lifeable.GetCurrentHpPercent();
            resourceBar.fillAmount = resourceable.GetCurrentResourcePercent();
            healthBarText.text = $"{lifeable.GetCurrentHp()}/{lifeable.GetMaxHp()}";
            
            handledPassives.Clear();

            for (var i = 0; i < 3; i++)
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

        private void ChangeAbilityIcon(int index, ActiveCapacity capacity)
        {
            ChangeAbilityIcon(index, capacity,activeIcons[index].control);
        }

        private void ChangeAbilityIcon(int index,ActiveCapacity capacity,InputControl control,int upgradeIndex = -1)
        {
            activeIcons[index].SetCapacity(capacity,control,upgradeIndex);
        }

        private void UpdateIcons(Champion champ)
        {
            var so = champ.currentSo;
            
            Portrait.sprite = so.portrait;

            foreach (var passiveCapacity in champ.passiveCapacitiesList)
            {
                AddPassiveIcon(passiveCapacity);
            }


            var inputMap = InputManager.PlayerMap;
            
            ChangeAbilityIcon(0,champ.capacityDict[champ.abilitiesIndexes[0]].capacity,inputMap.Capacity.Capacity0.controls[0],0);
            ChangeAbilityIcon(1,champ.capacityDict[champ.abilitiesIndexes[1]].capacity,inputMap.Capacity.Capacity1.controls[0],1);
            ChangeAbilityIcon(2,champ.capacityDict[champ.abilitiesIndexes[2]].capacity,inputMap.Capacity.Capacity2.controls[0],2);
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

                if (!passiveIcon.RemovePassive(_)) return;
                
                handledPassives.Remove(capacity);
                Destroy(passiveIcon.gameObject);

            }
        }

        
        
    }
}