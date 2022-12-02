using System.Collections.Generic;

namespace BehaviourTree
{
    public enum NodeState
    {
        Running,
        Success,
        Failure
    }
    public abstract class Node
    {
        public Node Parent;
        
        protected NodeState state;
        protected List<Node> children = new List<Node>();

        private Dictionary<string, object> data = new Dictionary<string, object>();

        public Node()
        {
            Parent = null;
        }

        public Node(List<Node> children)
        {
            foreach (Node child in children)
            {
                AttachNode(child);
            }
        }

        private void AttachNode(Node node)
        {
            node.Parent = this;
            children.Add(node);
        }

        public abstract NodeState Evaluate();

        public void SetDataInBlackboard(string key, object value)
        {
            data[key] = value;
        }

        public object GetData(string key)
        {
            object value = null;
            if (data.TryGetValue(key, out value))
                return value;

            Node node = Parent;
            while (node != null)
            {
                value = node.GetData(key);
                if (value != null)
                    return value;
                node = node.Parent;
            }

            return null;
        }

        public bool ClearData(string key)
        {
            if (data.ContainsKey(key))
            {
                data.Remove(key);
                return true;
            }

            Node node = Parent;
            while (node != null)
            {
                bool cleared = node.ClearData(key);
                if (cleared)
                    return true;
                node = node.Parent;
            }

            return false;
        }
    }
}

