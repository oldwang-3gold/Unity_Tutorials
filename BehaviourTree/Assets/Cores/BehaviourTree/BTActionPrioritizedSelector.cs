using System.Collections;
using System.Collections.Generic;

namespace GGGBT
{
    // 优先选择节点，只要有一个子节点通过就返回finish
    // 相当于对一系列行为定义了优先级，从左到右
    public class BTActionPrioritizedSelector : BTAction
    {
        protected class BTActionPrioritizedSelectorContext : BTActionContext
        {
            internal int currentSelectedIndex;
            internal int lastSelectedIndex;

            public BTActionPrioritizedSelectorContext()
            {
                currentSelectedIndex = -1;
                lastSelectedIndex = -1;
            }
        }

        public BTActionPrioritizedSelector() : base(-1){}

        protected override bool OnEvaluate(BTWorkingData wData)
        {
            var thisContext = GetContext<BTActionPrioritizedSelectorContext>(wData);
            thisContext.currentSelectedIndex = -1;
            int childCount = GetChildCount();
            for (int i = 0; i < childCount; ++i)
            {
                BTAction node = GetChild<BTAction>(i);
                if (node.Evaluate(wData))
                {
                    thisContext.currentSelectedIndex = i;
                    return true;
                }
            }
            return false;
        }

        protected override int OnUpdate(BTWorkingData wData)
        {
            var thisContext = GetContext<BTActionPrioritizedSelectorContext>(wData);
            int runningState = BTRunningStatus.FINISHED;
            if (thisContext.currentSelectedIndex != thisContext.lastSelectedIndex)
            {
                if (IsIndexValid(thisContext.lastSelectedIndex))
                {
                    // 每完成一个节点
                    BTAction node = GetChild<BTAction>(thisContext.lastSelectedIndex);
                    node.Transition(wData);
                }
                thisContext.lastSelectedIndex = thisContext.currentSelectedIndex;
            }
            if (IsIndexValid(thisContext.lastSelectedIndex))
            {
                BTAction node = GetChild<BTAction>(thisContext.lastSelectedIndex);
                runningState = node.Update(wData);
                if (BTRunningStatus.IsFinished(runningState))
                {
                    thisContext.lastSelectedIndex = -1;
                }
            }
            return runningState;
        }

        protected override void OnTransition(BTWorkingData wData)
        {
            var thisContext = GetContext<BTActionPrioritizedSelectorContext>(wData);
            BTAction node = GetChild<BTAction>(thisContext.lastSelectedIndex);
            if (node != null)
            {
                node.Transition(wData);
            }
            thisContext.lastSelectedIndex = -1;
        }
    }
}

