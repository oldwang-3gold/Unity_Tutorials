using System.Collections;
using System.Collections.Generic;

namespace GGGBT
{
    // ÿ����Ϊ�ڵ��ڲ�ά�����߼�����
    public class BTActionContext { }

    public abstract class BTAction : BTTreeNode
    {
        static private int sUNIQUEKEY = 0;
        private int GenUniqueKey()
        {
            if (sUNIQUEKEY >= int.MaxValue)
            {
                sUNIQUEKEY = 0;
            }
            else
            {
                sUNIQUEKEY++;
            }
            return sUNIQUEKEY;
        }

        protected int _uniqueKey;
        protected BTPrecondition _precondition;

        public BTAction(int maxChildCount) : base(maxChildCount) 
        {
            _uniqueKey = GenUniqueKey();
        }

        // ��Ϊ��ÿ֡����ȥ�ж��Ƿ���Դ�����ǰ��Ϊ�ڵ�
        public bool Evaluate(BTWorkingData wData)
        {
            return (_precondition == null || _precondition.IsTrue(wData)) && OnEvaluate(wData);
        }

        public int Update(BTWorkingData wData)
        {
            return OnUpdate(wData);
        }

        public void Transition(BTWorkingData wData)
        {
            OnTransition(wData);
        }

        public BTAction SetPrecondition(BTPrecondition precondition)
        {
            _precondition = precondition;
            return this;
        }

        public override int GetHashCode()
        {
            return _uniqueKey;
        }

        protected T GetContext<T> (BTWorkingData wData) where T : BTActionContext, new()
        {
            int uniqueKey = GetHashCode();
            T thisContext;
            if (!wData.Context.ContainsKey(uniqueKey))
            {
                thisContext = new T();
                wData.Context.Add(uniqueKey, thisContext);
            }
            else
            {
                thisContext = (T)wData.Context[uniqueKey];
            }
            return thisContext;
        }

        protected virtual int OnUpdate(BTWorkingData wData)
        {
            return BTRunningStatus.FINISHED;
        }

        // ����ʵ��
        protected virtual bool OnEvaluate(BTWorkingData wData)
        {
            return true;
        }
        protected virtual void OnTransition(BTWorkingData wData)
        {

        }
    }
}

