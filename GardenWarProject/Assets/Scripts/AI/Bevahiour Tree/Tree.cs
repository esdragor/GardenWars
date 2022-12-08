using UnityEngine;

namespace BehaviourTree
{
    public abstract class Tree : MonoBehaviour
    {

        private Node origin = null;


        // private void Start()
        // {
        //     OnStart();
        // }

        public void OnStart()
        {
            origin = InitTree();
        }

        private void Update()
        {
            if (origin != null)
                origin.Evaluate(origin);
        }

        protected abstract Node InitTree();
    }
}