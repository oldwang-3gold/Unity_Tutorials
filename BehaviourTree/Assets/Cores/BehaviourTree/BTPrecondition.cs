using System.Collections;
using System.Collections.Generic;

namespace GGGBT
{
    public abstract class BTPrecondition : BTTreeNode
    {
        public BTPrecondition(int maxChildCount) : base(maxChildCount) { }

        public abstract bool IsTrue(BTWorkingData wData);
    }

    public abstract class BTPreconditionLeaf : BTPrecondition
    {
        public BTPreconditionLeaf() : base(0) { }
    }

    public abstract class BTPreconditionUnary : BTPrecondition
    {
        public BTPreconditionUnary(BTPrecondition lhs) : base(1)
        {
            AddChild(lhs);
        }
    }

    public abstract class BTPreconditionBinary : BTPrecondition
    {
        public BTPreconditionBinary(BTPrecondition lhs, BTPrecondition rhs) : base(2)
        {
            AddChild(lhs).AddChild(rhs);
        }
    }

    // 基础判断前置条件
    public class BTPreconditionTRUE : BTPreconditionLeaf
    {
        public override bool IsTrue(BTWorkingData wData)
        {
            return true;
        }
    }

    public class BTPreconditionFALSE : BTPreconditionLeaf
    {
        public override bool IsTrue(BTWorkingData wData)
        {
            return false;
        }
    }

    public class BTPreconditionNOT : BTPreconditionUnary
    {
        public BTPreconditionNOT(BTPrecondition lhs) : base(lhs)
        {

        }

        public override bool IsTrue(BTWorkingData wData)
        {
            return !GetChild<BTPrecondition>(0).IsTrue(wData);
        }
    }

    public class BTPreconditionAND : BTPreconditionBinary
    {
        public BTPreconditionAND(BTPrecondition lhs, BTPrecondition rhs) : base(lhs, rhs)
        {

        }

        public override bool IsTrue(BTWorkingData wData)
        {
            return GetChild<BTPrecondition>(0).IsTrue(wData) &&
                   GetChild<BTPrecondition>(1).IsTrue(wData);
        }
    }

    public class BTPreconditionOR : BTPreconditionBinary
    {
        public BTPreconditionOR(BTPrecondition lhs, BTPrecondition rhs) : base(lhs, rhs)
        {
            
        }

        public override bool IsTrue(BTWorkingData wData)
        {
            return GetChild<BTPrecondition>(0).IsTrue(wData) ||
                   GetChild<BTPrecondition>(1).IsTrue(wData);
        }
    }
}

