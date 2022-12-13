using System;
using System.Collections.Generic;
using Entities;
using UnityEngine;

public class OnCollideEffect : MonoBehaviour
{
    public event Action<Entity> OnEntityCollide;
    public event Action<Entity> OnEntityCollideFeedback;
    
    private bool stopAtWalls = false;
    private Entity target;

    public void Init(Entity _target, bool shouldStopAtWalls = false)
    {
        stopAtWalls = shouldStopAtWalls;
        target = _target;
    }

    private void OnCollisionEnter(Collision other)
    {
        var hitEntity = other.gameObject.GetComponent<Entity>();
        
        if (hitEntity == null && stopAtWalls)
        {
            OnEntityCollideFeedback?.Invoke(null);
            OnEntityCollideFeedback = null;
            gameObject.SetActive(false);
            return;
        }

        if (hitEntity != target) return;
        
        if (Entity.isMaster)
        {
            OnEntityCollide?.Invoke(target);
            OnEntityCollide = null;
        }
        OnEntityCollideFeedback?.Invoke(target);
        OnEntityCollideFeedback = null;
        gameObject.SetActive(false);
    }
}