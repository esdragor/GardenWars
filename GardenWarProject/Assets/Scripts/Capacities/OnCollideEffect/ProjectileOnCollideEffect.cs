using System;
using Entities;
using UnityEngine;

public class ProjectileOnCollideEffect : MonoBehaviour
{
    public event Action<Entity> OnEntityCollide;
    public event Action<Entity> OnEntityCollideFeedback;
    public event Action<Collision> OnCollide;
    public event Action<Collision> OnCollideFeedback;

    private void OnCollisionEnter(Collision other)
    {
        if (Entity.isMaster)
        {
            OnCollide?.Invoke(other);
            
        }
        OnCollideFeedback?.Invoke(other);
        
        var hitEntity = other.gameObject.GetComponent<Entity>();
        
        if(hitEntity == null) return;
        
        if (Entity.isMaster)
        {
            OnEntityCollide?.Invoke(hitEntity);
            
        }
        OnEntityCollideFeedback?.Invoke(hitEntity);
    }
    
    public void DestroyProjectile()
    {
        OnCollide = null;
        OnCollideFeedback = null;
        
        OnEntityCollide = null;
        OnEntityCollideFeedback = null;
        
        gameObject.SetActive(false);
    }
}