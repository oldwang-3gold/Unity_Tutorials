using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGGBT
{
    public class Composition : BTNode
    {
        public List<BTNode> childNodes;

        public Composition()
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
        }

        public override void Dispose()
        {
            base.Dispose();
            childNodes = null;
        }

        protected override void OnExit()
        {
            base.OnExit();
        }
    }
}


