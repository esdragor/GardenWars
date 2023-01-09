using System.Collections.Generic;
using System.Linq;
using Entities;
using GameStates;
using UnityEngine;

public class Bush : MonoBehaviour
{
    [SerializeField] private List<BushCollider> bushColliders = new List<BushCollider>();
    [SerializeField] private List<GameObject> objectsToHide = new List<GameObject>();
    [SerializeField] private Material normalMat;
    [SerializeField] private Material transparentMat;
    
    private List<Renderer> renderers = new List<Renderer>();

    private List<Entity> entitiesInside = new List<Entity>();

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        entitiesInside.Clear();
        foreach (var bushCollider in bushColliders)
        {
            bushCollider.LinkWithBush(this);
        }

        foreach (var childRenderer in objectsToHide.SelectMany(go => go.GetComponentsInChildren<Renderer>()))
        {
            renderers.Add(childRenderer);
            childRenderer.material = normalMat;
        }
    }
    
    public void EntityEnter(Entity entity)
    {
        if(!entitiesInside.Contains(entity)) entitiesInside.Add(entity);

        if (!HideBush()) return;
        
        foreach (var childRenderer in renderers)
        {
            childRenderer.material = transparentMat;
        }
    }
    
    public void EntityExit(Entity entity)
    {
        if(entitiesInside.Contains(entity)) entitiesInside.Remove(entity);

        if (HideBush()) return;
        
        foreach (var childRenderer in renderers)
        {
            childRenderer.material = normalMat;
        }
    }

    private bool HideBush()
    {
        if (entitiesInside.Count == 0) return false;
        foreach (var e in entitiesInside)
        {
            if (e.team == GameStateMachine.Instance.GetPlayerTeam()) return true;
        }
        return false;
    }
    
}
