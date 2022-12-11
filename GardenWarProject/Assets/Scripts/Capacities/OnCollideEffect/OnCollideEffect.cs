using System;
using System.Collections.Generic;
using Entities;
using UnityEngine;

public class OnCollideEffect : MonoBehaviour
{
    public event Action<Entity> OnEntityCollide;
    public event Action<Entity> OnEntityCollideFeedback;
    private Entity hitEntity;
    private List<Enums.Team> enemyTeams = new List<Enums.Team>();
    private bool canHitAlly = false;

    public void Init(Entity caster,bool canHitAllies = false)
    {
        enemyTeams.Clear();
        enemyTeams = caster.GetEnemyTeams();
        canHitAlly = canHitAllies;
    }
    
    private void OnCollisionEnter(Collision other)
    {
        hitEntity = other.gameObject.GetComponent<Entity>();
        if (hitEntity == null) return;
        if((canHitAlly && enemyTeams.Contains(hitEntity.team)) || !canHitAlly && !enemyTeams.Contains(hitEntity.team)) return;
        Debug.Log($"hitting {hitEntity}");
        if (Entity.isMaster)
        {
            OnEntityCollide?.Invoke(hitEntity);
            OnEntityCollide = null;
        }
        OnEntityCollideFeedback?.Invoke(hitEntity);
        OnEntityCollideFeedback = null;
        gameObject.SetActive(false); //TODO - link to pool
    }
}
