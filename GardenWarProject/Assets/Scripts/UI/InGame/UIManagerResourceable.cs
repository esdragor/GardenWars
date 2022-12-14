using Entities;
using Entities.FogOfWar;
using GameStates;
using UnityEngine;
using UIComponents;

public partial class UIManager
{
    [Header("ResourceBar Elements")]
    [SerializeField] private GameObject resourceBarPrefab;
    
    public void InstantiateResourceBarForEntity(Entity entity)
    {
        if (entity == null) return;
        if (entity.GetComponent<IResourceable>() == null) return;
        var canvasResource = Instantiate(resourceBarPrefab, entity.uiTransform.position + entity.uiOffset, Quaternion.identity, entity.uiTransform);
        entity.elementsToShow.Add(canvasResource);
        canvasResource.GetComponent<EntityResourceBar>().InitResourceBar(entity);
    }
}