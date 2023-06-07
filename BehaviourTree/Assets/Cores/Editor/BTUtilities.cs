using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGGBT
{
    public static class BTUtilities
    {
        public static int GetNodeUID()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            int uid = BitConverter.ToInt32(buffer, 0);
            return uid;
        }
    }
}

