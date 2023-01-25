using System;
using System.Threading.Tasks;
using Entities;
using Entities.Champion;
using GameStates;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace UIComponents
{
    public class EntityHealthBar : MonoBehaviour
    {
        [SerializeField] private Image healthBar;
        [SerializeField] private Image backHealthBar;
        [SerializeField] private Sprite[] ChampionColor;
        [SerializeField] private Sprite[] AllyColor;
        [SerializeField] private Sprite[] EnemyColor;
        [SerializeField] private Sprite[] EnemyChampionColor;
        [SerializeField] private TMP_Text textDamage;

        private bool isLocalPlayerAndChampion;

        private IActiveLifeable lifeable;
        private Camera cam;
        private byte index = 0;

        public void InitHealthBar(Entity entity)
        {
            isLocalPlayerAndChampion = entity is Champion champion && champion == GameStateMachine.Instance.GetPlayerChampion(); 
            
            lifeable = (IActiveLifeable)entity;

            transform.LookAt(transform.position + GameStateMachine.mainCam.transform.rotation * Vector3.forward,
                Camera.main.transform.rotation * Vector3.up);
            healthBar.fillAmount = lifeable.GetCurrentHpPercent();
            lifeable.OnSetCurrentHpFeedback += UpdateFillPercent;
            lifeable.OnSetCurrentHpPercentFeedback += UpdateFillPercentByPercent;
            lifeable.OnIncreaseCurrentHpFeedback += UpdateFillPercent;
            lifeable.OnDecreaseCurrentHpFeedback += UpdateFillPercent;
            lifeable.OnDecreaseCurrentHpFeedback += PrintDamage;
            lifeable.OnDecreaseCurrentHp += PrintDamage;
            lifeable.OnIncreaseMaxHpFeedback += UpdateFillPercent;
            lifeable.OnDecreaseMaxHpFeedback += UpdateFillPercent;

            cam = Camera.main;
            if (entity is Champion)
            {
                if (GameStateMachine.Instance.GetPlayerTeam() == entity.GetTeam())
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
                if (GameStateMachine.Instance.GetPlayerTeam() == entity.GetTeam())
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

        private void Update()
        {
            transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward,
                cam.transform.rotation * Vector3.up);
        }
    }
}