using BehaviourTree;
using Entities;
using GameStates;
using UnityEngine;

public class CheckEnemyInPOVRange : Node
{
    private int EnemyLayerMaskF = 1 << 6;
    private Transform MyTransform;
    private Entity MyEntity;
    private float rangeFOV = 5f;
    private int layerTargetFogOfWar = 1 << 29 | 1 << 30;

    private Node Root;

    public CheckEnemyInPOVRange(Node _Root, Entity entity, int enemyMaskByFunct, float _rangeFOV = 5f)
    {
        if (entity == null) return;
        MyTransform = entity.transform;
        rangeFOV = _rangeFOV;
        EnemyLayerMaskF = enemyMaskByFunct;
        Root = _Root;
        MyEntity = entity;
    }
    public override NodeState Evaluate(Node root)
    {
        if (Root == null) Root = root;
        Entity t = (Entity)Root.GetData("target");
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
                    if(Physics.Raycast(MyTransform.position, (entity.transform.position -MyTransform.position).normalized , out var hit, MyEntity.viewRange,
                           layerTargetFogOfWar))
                    {
                        if(hit.collider.gameObject.layer != 29) continue;
                    }

                    if (!MyEntity.GetEnemyTeams().Contains(entity.team)) continue;

                    IAttackable attackable = coll.GetComponent<IAttackable>();
                    if (attackable == null) continue;
                    
                    Root.SetDataInBlackboard("target", entity);
                    //animator.SetBool("Walking", true);
                    MyEntity.SetAnimatorTrigger("SpotEnemy");
                    state = NodeState.Success;
                    return state;
                }

                return NodeState.Failure;
            }
        }

        if (t != null && !t.gameObject.activeSelf)
        {
            Root.ClearData("target");
            state = NodeState.Failure;
        }
        if (t != null && Vector3.Distance(t.transform.position, MyTransform.position) < rangeFOV)
            return NodeState.Success;
        if (t != null) Root.ClearData("target");
        state = NodeState.Failure;
        return state;
    }
}