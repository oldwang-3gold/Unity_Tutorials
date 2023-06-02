using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGGBT
{
    // 次序节点，与门逻辑，任何一个子节点返回executing就不会继续
    public class BTActionSequence : BTAction
    {
        protected class BTActionSequenceContext : BTActionContext
        {
            internal int currentSelectedIndex;
            public BTActionSequenceContext()
            {
                currentSelectedIndex = -1;
            }
        }

        public BTActionSequence() : base(-1)
        {

        }

        protected override bool OnEvaluate(BTWorkingData wData)
        {
            var thisContext = GetContext<BTActionSequenceContext>(wData);
            int checkedNodeIndex = -1;
            if (IsIndexValid(thisContext.currentSelectedIndex))
            {
                checkedNodeIndex = thisContext.currentSelectedIndex;
            }
            else
            {
                checkedNodeIndex = 0;
            }
            if (IsIndexValid(checkedNodeIndex))
            {
                BTAction node = GetChild<BTAction>(checkedNodeIndex);
                if (node.Evaluate(wData))
                {
                    thisContext.currentSelectedIndex = checkedNodeIndex;
                    return true;
                }
            }
            return false;
        }

        protected override int OnUpdate(BTWorkingData wData)
        {
            var thisContext = GetContext<BTActionSequenceContext>(wData);
            int runningStatus = BTRunningStatus.FINISHED;
            BTAction node = GetChild<BTAction>(thisContext.currentSelectedIndex);
            runningStatus = node.Update(wData);
            if (BTRunningStatus.IsFinished(runningStatus))
            {
                thisContext.currentSelectedIndex++;
                if (IsIndexValid(thisContext.currentSelectedIndex))
                {
                    runningStatus = BTRunningStatus.EXECUTING;
                }
                else
                {
                    thisContext.currentSelectedIndex = -1;
                }
            }
            return runningStatus;
        }

        protected override void OnTransition(BTWorkingData wData)
        {
            var thisContext = GetContext<BTActionSequenceContext>(wData);
            BTAction node = GetChild<BTAction>(thisContext.currentSelectedIndex);
            if (node != null)
            {
                node.Transition(wData);
            }
            thisContext.currentSelectedIndex = -1;
        }
    }
}

