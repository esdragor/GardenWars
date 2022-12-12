using System;
using System.Collections.Generic;
using Entities;
using UnityEngine;

public class OnCollideEffect : MonoBehaviour
{
    public event Action<Entity> OnEntityCollide;
    public event Action<Entity> OnEntityCollideFeedback;

    private List<Enums.Team> enemyTeams = new List<Enums.Team>();
    private bool canHitAlly = false;
    private Entity target;

    public void Init(Entity caster, Entity _target, bool canHitAllies = false)
    {
        enemyTeams.Clear();
        enemyTeams = caster.GetEnemyTeams();
        canHitAlly = canHitAllies;
        target = _target;
    }

    private void OnCollisionEnter(Collision other)
    {
        Entity hitEntity = other.gameObject.GetComponent<Entity>();
        if (hitEntity == null)
        {
            OnEntityCollideFeedback?.Invoke(null);
            OnEntityCollideFeedback = null;
            gameObject.SetActive(false);
            return;
        }
        if ((canHitAlly && enemyTeams.Contains(hitEntity.team)) ||
            !canHitAlly && !enemyTeams.Contains(hitEntity.team)) return;
        if (hitEntity != target) return;
        if (Entity.isMaster)
        {
            OnEntityCollide?.Invoke(hitEntity);
            OnEntityCollide = null;
        }

        OnEntityCollideFeedback?.Invoke(hitEntity);
        OnEntityCollideFeedback = null;
    }
}