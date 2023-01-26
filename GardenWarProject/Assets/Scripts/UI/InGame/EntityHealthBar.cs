using System;
using System.Threading.Tasks;
using Entities;
using Entities.Champion;
using GameStates;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIComponents
{
    public class EntityHealthBar : MonoBehaviour
    {
        [SerializeField] private Image healthBar;
        private Transform backHealthTr;
        [SerializeField] private Image backHealthBar;
        [SerializeField] private Sprite[] ChampionColor;
        [SerializeField] private Sprite[] AllyColor;
        [SerializeField] private Sprite[] EnemyColor;
        [SerializeField] private Sprite[] EnemyChampionColor;
        [SerializeField] private TMP_Text textDamage;
        [SerializeField] private float championHeight = 2.5f;
        [SerializeField] private float minionHeight = 2.5f;
        [SerializeField] private Vector2 championSize;
        [SerializeField] private Vector2 minionSize;
        

        private bool isLocalPlayerAndChampion;
        private bool isChampion;
        
        private IActiveLifeable lifeable;
        private Entity entity;
        private Camera cam;
        private byte index = 0;

        private void Start()
        {
            backHealthTr = backHealthBar.transform;
        }

        private void Update()
        {
            if(lifeable == null) return;
            
            backHealthTr.position = cam.WorldToScreenPoint(entity.position + Vector3.up * (isChampion ? championHeight : minionHeight));
        }

        public void InitHealthBar(Entity ent)
        {
            entity = ent;
            
            isChampion = entity is Champion;
            
            isLocalPlayerAndChampion = isChampion && ent.entityIndex == GameStateMachine.Instance.GetPlayerChampion().entityIndex;

            lifeable = (IActiveLifeable)ent;

            healthBar.fillAmount = lifeable.GetCurrentHpPercent();
            lifeable.OnSetCurrentHpFeedback += UpdateFillPercent;
            lifeable.OnSetCurrentHpPercentFeedback += UpdateFillPercentByPercent;
            lifeable.OnIncreaseCurrentHpFeedback += UpdateFillPercent;
            lifeable.OnDecreaseCurrentHpFeedback += UpdateFillPercent;
            lifeable.OnDecreaseCurrentHpFeedback += PrintDamage;
            lifeable.OnDecreaseCurrentHp += PrintDamage;
            lifeable.OnIncreaseMaxHpFeedback += UpdateFillPercent;
            lifeable.OnDecreaseMaxHpFeedback += UpdateFillPercent;

            cam = GameStateMachine.mainCam;
            
            healthBar.GetComponent<RectTransform>().sizeDelta = isChampion ? championSize : minionSize;
            backHealthBar.GetComponent<RectTransform>().sizeDelta = isChampion ? championSize : minionSize;
            if (isChampion)
            {
                
                if (GameStateMachine.Instance.GetPlayerTeam() == ent.GetTeam())
                {
                    healthBar.sprite = ChampionColor[0];
                    backHealthBar.sprite = ChampionColor[1];
                }
                else
                {
                    healthBar.sprite = EnemyChampionColor[0];
                    backHealthBar.sprite = EnemyChampionColor[1];
                }
            }
            else
            {
                if (GameStateMachine.Instance.GetPlayerTeam() == ent.GetTeam())
                {
                    healthBar.sprite = AllyColor[0];
                    backHealthBar.sprite = AllyColor[1];
                }
                else
                {
                    healthBar.sprite = EnemyColor[0];
                    backHealthBar.sprite = EnemyColor[1];
                }
            }
        }

        private void UpdateFillPercentByPercent(float value)
        {
            healthBar.fillAmount = lifeable.GetCurrentHp() / lifeable.GetMaxHp();
        }

        private void UpdateFillPercent(float value)
        {
            healthBar.fillAmount = lifeable.GetCurrentHp() / lifeable.GetMaxHp();
        }

        private async void PrintDamage(float damage, int killerID)
        {
            if (damage < 1) return;
            if (isLocalPlayerAndChampion ||
                killerID == GameStateMachine.Instance.GetPlayerChampion().entityIndex)
            {
                textDamage.text = (-(int)damage).ToString();
                textDamage.gameObject.SetActive(true);
                index++;
                Debug.Log("PrintDamage to Champion");
                await Task.Delay(1000);
                index--;
                if (index <= 0)
                    textDamage.gameObject.SetActive(false);
            }
        }

        private void UpdateFillPercent(float value, int _)
        {
            healthBar.fillAmount = lifeable.GetCurrentHp() / lifeable.GetMaxHp();
        }

        public void Unlink()
        {
            lifeable.OnSetCurrentHpFeedback -= UpdateFillPercent;
            lifeable.OnSetCurrentHpPercentFeedback -= UpdateFillPercentByPercent;
            lifeable.OnIncreaseCurrentHpFeedback -= UpdateFillPercent;
            lifeable.OnDecreaseCurrentHpFeedback -= UpdateFillPercent;
            lifeable.OnDecreaseCurrentHpFeedback -= PrintDamage;
            lifeable.OnDecreaseCurrentHp -= PrintDamage;
            lifeable.OnIncreaseMaxHpFeedback -= UpdateFillPercent;
            lifeable.OnDecreaseMaxHpFeedback -= UpdateFillPercent;

            entity = null;
            lifeable = null;
        }
    }
}