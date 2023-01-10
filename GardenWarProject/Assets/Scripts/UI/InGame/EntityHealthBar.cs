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
        [SerializeField] private Color ChampionColor;
        [SerializeField] private Color AllyColor;
        [SerializeField] private Color EnemyColor;
        
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
            if (GameStateMachine.Instance.GetPlayerTeam() != entity.GetTeam())
            {
                healthBar.color = EnemyColor;
            }
            else
            {
                healthBar.color = (!entity.GetComponent<Champion>()) ? AllyColor : ChampionColor;
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