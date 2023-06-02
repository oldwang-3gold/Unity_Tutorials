using System.Collections;
using System.Collections.Generic;

namespace GGGBT
{
    public class BTTreeNode
    {
        private int _maxChildCount;
        private List<BTTreeNode> _children;

        public BTTreeNode(int maxChildCount = -1)
        {
            _children = new List<BTTreeNode>();
            _maxChildCount = maxChildCount;
            if (maxChildCount >= 0) 
            {
                _children.Capacity = maxChildCount;
            }
        }

        public BTTreeNode AddChild(BTTreeNode child)
        {
            if (_maxChildCount >= 0 && _children.Count >= _maxChildCount)
            {
                return this;
            }
            _children.Add(child);
            return this;
        }

        public int GetChildCount()
        {
            return _children.Count;
        }

        public bool IsIndexValid(int index)
        {
            return index >= 0 && index < _children.Count;
        }

        public T GetChild<T>(int index) where T : BTTreeNode
        {
            if (index < 0 || index >= _children.Count)
            {
                return null;
            }
            return (T)_children[index];
        }
    }
}

