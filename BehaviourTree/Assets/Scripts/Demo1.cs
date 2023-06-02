using UnityEngine;
using GGGBT;

public class Demo1
{
    private static BTAction _bevTreeDemo1;
    static public BTAction CreateTree()
    {
        _bevTreeDemo1 = new BTActionPrioritizedSelector();
        _bevTreeDemo1
            .AddChild(new BTActionSequence()
                .SetPrecondition(new BTPreconditionNOT(new CON_HasReachedTarget()))
                .AddChild(new ACT_MoveTo()))
            .AddChild(new ACT_Attack());
        return _bevTreeDemo1;
    }
}

class AIEntityWorkingData : BTWorkingData
{
    public AIEntity Entity { get; set; }
    public Transform EntityTF { get; set; }
    public float DeltaTime { get; set; }
}

class CON_HasReachedTarget : BTPreconditionLeaf
{
    public override bool IsTrue(BTWorkingData wData)
    {
        AIEntityWorkingData thisData = wData.As<AIEntityWorkingData>();
        Vector3 targetPos = TMathUtils.Vector3ZeroY(thisData.Entity.GetBBValue<Vector3>(AIBBConst.TARGET_MOVING_POSITION, Vector3.zero));
        Vector3 currentPos = TMathUtils.Vector3ZeroY(thisData.EntityTF.position);
        return TMathUtils.GetDistance2D(currentPos, targetPos) < 1f;
    }
}

class ACT_MoveTo : BTActionLeaf
{
    protected override void OnEnter(BTWorkingData wData)
    {
        Debug.Log("ACT_RUN ENTER!!!");
    }

    protected override int OnExecute(BTWorkingData wData)
    {
        AIEntityWorkingData thisData = wData.As<AIEntityWorkingData>();
        Vector3 targetPos = TMathUtils.Vector3ZeroY(thisData.Entity.GetBBValue<Vector3>(AIBBConst.TARGET_MOVING_POSITION, Vector3.zero));
        Vector3 currentPos = TMathUtils.Vector3ZeroY(thisData.EntityTF.position);
        float distToTarget = TMathUtils.GetDistance2D(currentPos, targetPos);
        if (distToTarget < 1f)
        {
            thisData.EntityTF.position = targetPos;
            return BTRunningStatus.FINISHED;
        }
        else
        {
            int ret = BTRunningStatus.EXECUTING;
            Vector3 dir = TMathUtils.GetDirection2D(currentPos, targetPos);
            float movingStep = thisData.Entity.MovingSpeed * thisData.DeltaTime;
            if (movingStep > distToTarget)
            {
                movingStep = distToTarget;
                ret = BTRunningStatus.FINISHED;
            }
            thisData.EntityTF.position = thisData.EntityTF.position + dir * movingStep;
            return ret;
        }
    }
}

class ACT_Attack : BTActionLeaf
{
    class UserContextData
    {
        internal float attackTime;
    }

    protected override void OnEnter(BTWorkingData wData)
    {
        Debug.Log("Enter Attack");
        AIEntityWorkingData thisData = wData.As<AIEntityWorkingData>();
        UserContextData userData = GetUserContextData<UserContextData>(wData);
        userData.attackTime = 5f;
    }

    protected override int OnExecute(BTWorkingData wData)
    {
        AIEntityWorkingData thisData = wData.As<AIEntityWorkingData>();
        UserContextData userData = GetUserContextData<UserContextData>(wData);
        if (userData.attackTime > 0)
        {
            userData.attackTime -= thisData.DeltaTime;
            if (userData.attackTime <= 0)
            {
                Debug.Log("Attack success!!!");
                return BTRunningStatus.FINISHED;
            }
        }
        return BTRunningStatus.EXECUTING;
    }
}
