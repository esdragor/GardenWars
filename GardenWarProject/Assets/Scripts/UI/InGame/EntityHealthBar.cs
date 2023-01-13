using System;
using Entities;
using Entities.Champion;
using GameStates;
using UnityEngine;
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
        
        private IActiveLifeable lifeable;
        private Camera cam;
        
        public void InitHealthBar(Entity entity)
        {
            lifeable = (IActiveLifeable)entity;
            
            transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
            healthBar.fillAmount = lifeable.GetCurrentHpPercent();
            lifeable.OnSetCurrentHpFeedback += UpdateFillPercent;
            lifeable.OnSetCurrentHpPercentFeedback += UpdateFillPercentByPercent;
            lifeable.OnIncreaseCurrentHpFeedback += UpdateFillPercent;
            lifeable.OnDecreaseCurrentHpFeedback += UpdateFillPercent;
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
            healthBar.fillAmount = lifeable.GetCurrentHp()/lifeable.GetMaxHp();
        }
        
        private void UpdateFillPercent(float value)
        {
            healthBar.fillAmount = lifeable.GetCurrentHp()/lifeable.GetMaxHp();
        }
    
        private void UpdateFillPercent(float value,int _)
        {
            healthBar.fillAmount = lifeable.GetCurrentHp()/lifeable.GetMaxHp();
        }

        private void Update()
        {
           transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);
        }
    }
}