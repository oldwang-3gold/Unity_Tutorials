using System.Collections;
using System.Collections.Generic;

namespace GGGBT
{
    public abstract class BTActionLeaf : BTAction
    {
        // 具体行为的状态变化，与BTRunningStates的不同
        private const int ACTION_READY = 0;
        private const int ACTION_RUNNING = 1;
        private const int ACTION_FINISHED = 2;

        class BTActionLeafContext : BTActionContext
        {
            internal int status;
            internal bool needExit;
            private object _userData;

            public BTActionLeafContext()
            {
                status = ACTION_READY;
                needExit = false;
                _userData = null;
            }
            
            public T GetUserData<T>() where T : class, new()
            {
                if (_userData == null)
                {
                    _userData = new T();
                }
                return (T)_userData;
            }
        }

        public BTActionLeaf() : base(0) { }

        // 不允许子类再继承修改方法
        protected sealed override int OnUpdate(BTWorkingData wData)
        {
            // 每个具体行为叶子节点的状态机
            // ACTION_READY -> ACTION_RUNNING -> ACTION_FINISHED -> ACTION_READY
            // 各自按需实现OnEnter、OnExecute、OnExit
            int runningState = BTRunningStatus.FINISHED;
            BTActionLeafContext thisContext = GetContext<BTActionLeafContext>(wData);
            if (thisContext.status == ACTION_READY)
            {
                OnEnter(wData);
                thisContext.needExit = true;
                thisContext.status = ACTION_RUNNING;
            }
            if (thisContext.status == ACTION_RUNNING)
            {
                runningState = OnExecute(wData);
                if (BTRunningStatus.IsFinished(runningState))
                {
                    thisContext.status = ACTION_FINISHED;
                }
            }
            if (thisContext.status == ACTION_FINISHED)
            {
                if (thisContext.needExit)
                {
                    OnExit(wData, runningState);
                }
                thisContext.status = ACTION_READY;
                thisContext.needExit = false;
            }
            return runningState;
        }

        // 行为结束时进行节点的转移逻辑
        protected sealed override void OnTransition(BTWorkingData wData)
        {
            BTActionLeafContext thisContext = GetContext<BTActionLeafContext>(wData);
            if (thisContext.needExit)
            {
                OnExit(wData, BTRunningStatus.TRANSITION);
            }
            thisContext.status = ACTION_READY;
            thisContext.needExit = false;
        }

        protected T GetUserContextData<T> (BTWorkingData wData) where T : class, new()
        {
            return GetContext<BTActionLeafContext>(wData).GetUserData<T>();
        }

        // 子类实现
        protected virtual void OnEnter(BTWorkingData wData)
        {

        }

        protected virtual int OnExecute(BTWorkingData wData)
        {
            return BTRunningStatus.FINISHED;
        }

        protected virtual void OnExit(BTWorkingData wData, int runningStatus)
        {

        }

    }
}

