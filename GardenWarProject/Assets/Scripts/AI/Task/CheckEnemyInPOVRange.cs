using BehaviourTree;
using Entities;
using GameStates;
using UnityEngine;

public class CheckEnemyInPOVRange : Node
{
    private int EnemyLayerMaskF = 1 << 6;
    private Transform MyTransform;

    private float rangeFOV = 5f;
    //private Animator animator;

    public CheckEnemyInPOVRange(Transform trans, int enemyMaskByFunct, float _rangeFOV = 5f)
    {
        MyTransform = trans;
        //animator = getComponent<Animator>();
        rangeFOV = _rangeFOV;
        EnemyLayerMaskF =  enemyMaskByFunct;
    }

    public override NodeState Evaluate()
    {
        Entity t = (Entity)Parent.Parent.GetData("target");
        if (t == null)
        {
            Collider[] colliders = Physics.OverlapSphere(MyTransform.position, rangeFOV, EnemyLayerMaskF);

            if (colliders.Length > 1)
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i].gameObject == MyTransform.gameObject) return NodeState.Failure;
                    Entity entity = colliders[i].GetComponent<Entity>();
                    
                    if (!entity ) return NodeState.Failure;
                    
                    IAttackable attackable = colliders[i].GetComponent<IAttackable>();
                    
                    if (attackable == null) return NodeState.Failure;
                    
                    if (entity.team == GameStateMachine.Instance.GetPlayerTeam()) return NodeState.Failure;
                    
                    Parent.Parent.SetDataInBlackboard("target", entity);
                        //animator.SetBool("Walking", true);
                        state = NodeState.Success;
                        return state;
                }
            }
            state = NodeState.Failure;
            return state;
        }
        else
        {
            if (Vector3.Distance(t.transform.position, MyTransform.position) > rangeFOV)
            {
                Parent.Parent.ClearData("target");
                state = NodeState.Failure;
                return state;
            }
        }
        state = NodeState.Success;
        return state;
    }
}
