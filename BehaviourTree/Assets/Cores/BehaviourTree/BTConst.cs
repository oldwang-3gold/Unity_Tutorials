using System.Collections;
using System.Collections.Generic;

namespace GGGBT
{
    public class BTRunningStatus
    {
        public const int EXECUTING = 0;
        public const int FINISHED = 1;
        public const int TRANSITION = 2;

        public const int USER_EXECUTING = 100;

        public const int USER_FINISHED = 1000;

        static public bool IsOK(int runningStatus)
        {
            return runningStatus == FINISHED ||
                runningStatus >= USER_FINISHED;
        }

        static public bool IsError(int runningStatus)
        {
            return runningStatus < 0;
        }

        static public bool IsFinished(int runningStatus)
        {
            return IsOK(runningStatus) || IsError(runningStatus);
        }

        static public bool IsExecuting(int runningStatus)
        {
            return !IsFinished(runningStatus);
        }
    }
}

