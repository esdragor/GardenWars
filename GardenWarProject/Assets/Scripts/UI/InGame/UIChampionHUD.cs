using System.Collections.Generic;
using System.Linq;
using Entities;
using Entities.Capacities;
using Entities.Champion;
using GameStates;
using UnityEngine;
using UnityEngine.UI;

namespace UIComponents
{
    public class UIChampionHUD : MonoBehaviour
    {
        [Header("Passive")]
        [SerializeField] private UIPassiveIcon passiveIconPrefab;

        [SerializeField] private Transform positivePassiveParent;
        [SerializeField] private Transform negativePassiveParent;
        
        [Header("Components")]
        [SerializeField] private Image Portrait;
        [SerializeField] private Image healthBar;
        [SerializeField] private Image resourceBar;
        [SerializeField] private Image spellPassive;
        [SerializeField] private Image spellOne;
        [SerializeField] private Image spellTwo;
        [SerializeField] private Image spellUltimate;
        [SerializeField] private Image spellPassiveCooldown;
        [SerializeField] private Image spellOneCooldown;
        [SerializeField] private Image spellTwoCooldown;
        [SerializeField] private Image spellUltimateCooldown;
        
        private Champion champion;
        private IResourceable resourceable;
        private IActiveLifeable lifeable;
        private ICastable castable;
        private SpellHolder passiveHolder;
        private Dictionary<byte, SpellHolder> spellHolderDict = new Dictionary<byte, SpellHolder>();

        private Dictionary<PassiveCapacity, UIPassiveIcon> handledPassives =
            new Dictionary<PassiveCapacity, UIPassiveIcon>();

        public class SpellHolder
        {
            public Image spellIcon;
            public Image spellCooldown;

            public void Setup(Sprite image)
            {
                spellIcon.sprite = image;
                spellCooldown.fillAmount = 0;
            }

            public void ChangeIcon(Sprite image)
            {
                spellIcon.sprite = image;
            }

            public void StartTimer(float coolDown)
            {
                var timer = 0.0;
                var tckRate = GameStateMachine.Instance.tickRate;

                GameStateMachine.Instance.OnTick += Tick;

                void Tick()
                {
                    timer += 1.0 / tckRate;
                    spellCooldown.fillAmount = 1 - (float) (timer / coolDown);
                    if (!(timer > coolDown)) return;
                    GameStateMachine.Instance.OnTick -= Tick;
                    spellCooldown.fillAmount = 0;
                }
            }
        }

        public void InitHUD(Champion newChampion)
        {
            champion = newChampion;
            lifeable = champion.GetComponent<IActiveLifeable>();
            resourceable = champion.GetComponent<IResourceable>();
            castable = champion.GetComponent<ICastable>();

            healthBar.fillAmount = lifeable.GetCurrentHpPercent();
            resourceBar.fillAmount = resourceable.GetCurrentResourcePercent();
            
            handledPassives.Clear();
            
            LinkToEvents();
            UpdateIcons(champion);
        }

        private void InitHolders()
        {
            var so = champion.currentSo;
            spellPassive.sprite = champion.passiveCapacitiesList[0].AssociatedPassiveCapacitySO().icon;
            spellOne.sprite = so.activeCapacities[0].icon;
            spellTwo.sprite = so.activeCapacities[1].icon;
            spellUltimate.sprite = so.activeCapacities[2].icon;
        }

        private void LinkToEvents()
        {
            castable.OnCastFeedback += UpdateCooldown;

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

            champion.OnPassiveCapacityAddedFeedback += AddPassiveIcon;
        }

        private void UpdateIcons(Champion champion)
        {
            var so = champion.currentSo;

            passiveHolder = new SpellHolder
            {
                spellIcon = spellPassive,
                spellCooldown = spellPassiveCooldown
            };
            var spellOneHolder = new SpellHolder
            {
                spellIcon = spellOne,
                spellCooldown = spellOneCooldown
            };
            var spellTwoHolder = new SpellHolder
            {
                spellIcon = spellTwo,
                spellCooldown = spellTwoCooldown
            };
            var ultimateHolder = new SpellHolder
            {
                spellIcon = spellUltimate,
                spellCooldown = spellUltimateCooldown
            };
            spellHolderDict.Add(so.activeCapacitiesIndexes[0], spellOneHolder);
            //spellHolderDict.Add(so.activeCapacitiesIndexes[1], spellTwoHolder);
            if (!spellHolderDict.ContainsKey(so.activeCapacitiesIndexes[2]))
                spellHolderDict.Add(so.activeCapacitiesIndexes[2], ultimateHolder);
            else Debug.Log("A FIXE, CA BUG ");

            if (so.passiveCapacities.Length > 0)
                passiveHolder.Setup(so.passiveCapacities[0].icon);
            spellOneHolder.Setup(so.activeCapacities[0].icon);
            spellTwoHolder.Setup(so.activeCapacities[1].icon);
            ultimateHolder.Setup(so.activeCapacities[2].icon);

            Portrait.sprite = so.portrait;

            foreach (var passiveCapacity in champion.passiveCapacitiesList)
            {
                AddPassiveIcon(passiveCapacity);
            }
        }

        private void UpdateCooldown(byte capacityIndex, int intArray, Vector3 vectors)
        {
            //spellHolderDict[capacityIndex].StartTimer(CapacitySOCollectionManager.GetActiveCapacitySOByIndex(capacityIndex).cooldown) ;
        }

        private void UpdateFillPercentByPercentHealth(float value)
        {
            healthBar.fillAmount = lifeable.GetCurrentHp() / lifeable.GetMaxHp();
        }

        private void UpdateFillPercentHealth(float value)
        {
            healthBar.fillAmount = lifeable.GetCurrentHp() / lifeable.GetMaxHp();
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

            var passiveIcon = Instantiate(passiveIconPrefab, parent);

            handledPassives.Add(capacity,passiveIcon);
            
            passiveIcon.LinkWithChampion(champion,capacity,RemovePassiveIcon);
            
            void RemovePassiveIcon(Entity _)
            {
                if (passiveIcon.RemovePassive(_))
                {
                    handledPassives.Remove(capacity);
                    Destroy(passiveIcon.gameObject);
                }
            }
        }

        
        
    }
}