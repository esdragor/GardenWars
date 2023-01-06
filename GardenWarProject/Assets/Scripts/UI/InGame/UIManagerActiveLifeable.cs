using Entities;
using GameStates;
using UnityEngine;
using UIComponents;

public partial class UIManager
{
    [Header("HealthBar Elements")]
    [SerializeField] private GameObject healthBarPrefab;

    public void InstantiateHealthBarForEntity(Entity entity)
    {
        if (entity == null) return;
        if (entity.GetComponent<IActiveLifeable>() == null) return;
        var canvasHealth = Instantiate(healthBarPrefab, entity.uiTransform.position + entity.uiOffset, Quaternion.identity, entity.uiTransform);
        entity.elementsToShow.Add(canvasHealth);
        if (entity.team != gsm.GetPlayerTeam() && entity.team != Enums.Team.Neutral)
        {
            canvasHealth.SetActive(false);
        }
        canvasHealth.GetComponent<EntityHealthBar>().InitHealthBar(entity);
      
    }
}