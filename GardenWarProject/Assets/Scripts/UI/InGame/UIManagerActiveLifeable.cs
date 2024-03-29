using Entities;
using Entities.Champion;
using ExitGames.Client.Photon;
using GameStates;
using UnityEngine;
using UIComponents;

public partial class UIManager
{
    [Header("HealthBar Elements")]
    [SerializeField] private EntityHealthBar healthBarPrefab;

    public void InstantiateHealthBarForEntity(Entity entity)
    {
        if (entity == null) return;
        if (entity.GetComponent<IActiveLifeable>() == null) return;
        var canvasHealth = Instantiate(healthBarPrefab, entity.uiTransform.position + entity.uiOffset, Quaternion.identity,null);
        var canvasGo = canvasHealth.gameObject;
        entity.elementsToShow.Add(canvasGo);
        
        canvasGo.SetActive(true);
        if (entity.team != gsm.GetPlayerTeam() && entity.team != Enums.Team.Neutral)
        {
            canvasGo.SetActive(false);
        }
        
        canvasHealth.InitHealthBar(entity);

        entity.OnShowElementFeedback += ShowBar;
        entity.OnHideElementFeedback += HideBar;
        
        if(entity is Champion) return;
        var deadable = entity.GetComponent<IDeadable>();
        if(deadable != null) deadable.OnDieFeedback += Unlink;
        
        void ShowBar()
        {
            canvasGo.SetActive(true);
        }

        void HideBar()
        {
            canvasGo.SetActive(false);
        }

        void Unlink(int _)
        {
            canvasGo.SetActive(false);
            
            entity.OnShowElementFeedback -= ShowBar;
            entity.OnHideElementFeedback -= HideBar;
            if(!(entity is Champion)) deadable.OnDieFeedback -= Unlink;
            entity.elementsToShow.Remove(canvasGo);
            canvasGo.SetActive(false);
            
            canvasHealth.Unlink();
        }
        
    }
}