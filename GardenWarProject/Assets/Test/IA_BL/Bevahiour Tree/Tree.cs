using UnityEngine;

namespace BehaviourTree
{
    public abstract class Tree : MonoBehaviour
    {
        private Node origin = null;

        protected void Start()
        {
            origin = InitTree();
        }

        private void Update()
        {
            if (origin != null)
                origin.Evaluate();
        }

        protected abstract Node InitTree();
    }  
}

