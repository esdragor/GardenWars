using BehaviourTree;
using UnityEngine;

public class CheckEnemyInPOVRange : Node
{
    private static int EnemyLayerMaskF = 1 << 6;
    


    private Transform MyTransform;

    private float rangeFOV = 5f;
    //private Animator animator;

    public CheckEnemyInPOVRange(Transform trans, LayerMask enemyMaskBySerialize, int enemyMaskByFunct, float _rangeFOV = 5f)
    {
        MyTransform = trans;
        //animator = getComponent<Animator>();
        rangeFOV = _rangeFOV;
        EnemyLayerMaskF =  enemyMaskByFunct;
    }

    public override NodeState Evaluate()
    {
        object t = GetData("target");
        if (t == null)
        {
            Collider[] colliders = Physics.OverlapSphere(MyTransform.position, rangeFOV, EnemyLayerMaskF);

            if (colliders.Length > 1)
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i].gameObject != MyTransform.gameObject && colliders[i].gameObject != MyTransform.GetChild(0).gameObject)
                    {
                        Parent.SetDataInBlackboard("target", colliders[i].transform);
                        //animator.SetBool("Walking", true);
                        state = NodeState.Success;
                        return state;
                    }
                }

            }

            state = NodeState.Failure;
            return state;
        }

        state = NodeState.Success;
        return state;
    }
}
