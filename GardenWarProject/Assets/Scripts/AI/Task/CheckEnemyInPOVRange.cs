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

    private Node Root;
    //private Animator animator;

    public CheckEnemyInPOVRange(Node _Root, Entity entity, int enemyMaskByFunct, float _rangeFOV = 5f)
    {
        MyTransform = entity.transform;
        //animator = getComponent<Animator>();
        rangeFOV = _rangeFOV;
        EnemyLayerMaskF = enemyMaskByFunct;
        Root = _Root;
        MyEntity = entity;
    }

    public override NodeState Evaluate()
    {
        if (Root == null) return NodeState.Failure;
        Entity t = (Entity)Root.GetData("target");
        if (t == null)
        {
            Collider[] colliders = Physics.OverlapSphere(MyTransform.position, rangeFOV, EnemyLayerMaskF);

            if (colliders.Length > 1)
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i].gameObject == MyTransform.gameObject) continue;
                    Entity entity = colliders[i].GetComponent<Entity>();
                    if (!entity) continue;
                    if (!MyEntity.GetEnemyTeams().Contains(entity.team)) continue;

                    IAttackable attackable = colliders[i].GetComponent<IAttackable>();
                    if (attackable == null) continue;
                    Root.SetDataInBlackboard("target", entity);
                    //animator.SetBool("Walking", true);
                    state = NodeState.Success;
                    return state;
                }

                return NodeState.Failure;
            }
        }
        
        if (t != null && Vector3.Distance(t.transform.position, MyTransform.position) < rangeFOV)
            return NodeState.Success;
        if (t != null) Root.ClearData("target");
        state = NodeState.Failure;
        return state;
    }
}