using BehaviourTree;
using Entities;
using GameStates;
using UnityEngine;
using UnityEngine.Rendering;
using Tree = BehaviourTree.Tree;

public class CheckEnemyInPOVRange : Node
{
    private int EnemyLayerMaskF = 1 << 6;
    private Transform MyTransform;
    private Entity MyEntity;
    private float rangeFOV = 5f;
    private int layerTargetFogOfWar = 1 << 29 | 1 << 30;

    private Tree Root;
    
    private float maxRangeOfLane;

    public CheckEnemyInPOVRange(Tree _Root, Entity entity, int enemyMaskByFunct, float _maxRangeOfLane, float _rangeFOV)
    {
        if (entity == null) return;
        MyTransform = entity.transform;
        rangeFOV = _rangeFOV;
        EnemyLayerMaskF = enemyMaskByFunct;
        Root = _Root;
        MyEntity = entity;
        maxRangeOfLane = _maxRangeOfLane;
    }

    public override NodeState Evaluate(Node root)
    {
        if (!MyEntity) return NodeState.Failure; 
        Entity t = (Entity)Root.getOrigin().GetData("target");
        if (t == null)
        {
            Collider[] colliders = Physics.OverlapSphere(MyTransform.position, rangeFOV, EnemyLayerMaskF);

            if (colliders.Length > 1)
            {
                foreach (var coll in colliders)
                {
                    if (coll.gameObject == MyTransform.gameObject) continue;
                    var entity = coll.GetComponent<Entity>();
                    if (!entity) continue;
                    if (!entity.isVisible) continue;
                    Vector3 start = MyTransform.position;
                    start.y = 0.7f;
                    Vector3 end = entity.transform.position;
                    end.y = 0.7f;
                    if (Physics.Raycast(start,
                            (end - start).normalized, out var hit,
                            MyEntity.viewRange,
                            layerTargetFogOfWar))
                    {
                        if (hit.collider.gameObject.layer != 29) continue;
                    }
                    else
                    {
                        continue;
                    }

                    if (!MyEntity.GetEnemyTeams().Contains(entity.team) || entity.team == Enums.Team.Neutral) continue;

                    IAttackable attackable = coll.GetComponent<IAttackable>();
                    if (attackable == null) continue;

                    IDeadable deadable = coll.GetComponent<IDeadable>();
                    if (deadable == null) continue;
                    if (!deadable.IsAlive()) continue;
                    if (entity.IsInsideBush) continue;
                    
                    Minion minion = MyEntity as Minion;
                    
                    if (minion && 
                        Vector3.Distance(minion.lastCheckpoint, entity.transform.position) > maxRangeOfLane) continue;

                    Root.getOrigin().SetDataInBlackboard("target", entity);
                    state = NodeState.Success;
                    return state;
                }

                return NodeState.Failure;
            }
        }
        
        if ((t != null && !(((IDeadable)t).IsAlive())) ||
            t != null && t.IsInsideBush || 
             t != null && MyEntity is Minion &&  Vector3.Distance((MyEntity as Minion).lastCheckpoint, MyTransform.position) > maxRangeOfLane
            )
        {
            Root.getOrigin().ClearData("target");
            if (MyEntity is Tower)
            {
                (MyEntity as Tower).TargetLost();
            }
            return NodeState.Failure;
        }

        if (t != null && Vector3.Distance(t.transform.position, MyTransform.position) < rangeFOV)
        {
            if (MyEntity is Tower)
            {
                (MyEntity as Tower).TargetSeen(t.transform);
            }
            return NodeState.Success;
        }
        Root.getOrigin().ClearData("target");
        if (MyEntity is Tower)
        {
            (MyEntity as Tower).TargetLost();
        }

        return NodeState.Failure;
    }
}