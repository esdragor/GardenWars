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
        if (entity == null) return;
        MyTransform = entity.transform;
        //animator = getComponent<Animator>();
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
                    Entity entity = coll.GetComponent<Entity>();
                    if (!entity) continue;
                    if (!MyEntity.GetEnemyTeams().Contains(entity.team) || entity.team == Enums.Team.NotAssigned) continue;

                    IAttackable attackable = coll.GetComponent<IAttackable>();
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