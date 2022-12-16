using System.Collections.Generic;
using Entities;
using GameStates;
using UnityEngine;

public class Bush : MonoBehaviour
{
    [SerializeField] private List<BushCollider> bushColliders = new List<BushCollider>();
    [SerializeField] private List<GameObject> objectsToHide = new List<GameObject>();

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
    }
    
    public void EntityEnter(Entity entity)
    {
        if(!entitiesInside.Contains(entity)) entitiesInside.Add(entity);
        foreach (var obj in objectsToHide)
        {
            obj.SetActive(!HideBush());
        }
    }
    
    public void EntityExit(Entity entity)
    {
        if(entitiesInside.Contains(entity)) entitiesInside.Remove(entity);
        foreach (var obj in objectsToHide)
        {
            obj.SetActive(!HideBush());
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
