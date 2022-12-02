using System;
using UnityEngine;

namespace BehaviourTree
{
    public abstract class Tree : MonoBehaviour
    {
        public bool NotInstantiated = false;

        private Node origin = null;


        private void Start()
        {
            if (NotInstantiated)
                OnStart();
        }

        public void OnStart()
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