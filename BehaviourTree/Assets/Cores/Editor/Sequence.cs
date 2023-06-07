using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGGBT
{
    public class Sequence : Composition
    {
        protected int _currentNodeIndex;

        public Sequence() { }

        public override void OnCreate()
        {
            base.OnCreate();
            _currentNodeIndex = 0;
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            _currentNodeIndex = 0;
        }

        protected override void OnExit()
        {
            base.OnExit();
            _currentNodeIndex = 0;
        }

        public override void Dispose()
        {
            base.Dispose();
            _currentNodeIndex = 0;
        }
    }
}

