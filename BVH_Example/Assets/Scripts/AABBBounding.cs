using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AABBBoundingExtension
{
    public static bool CheckAABB(this AABBBounding a, AABBBounding b)
    {
        float AMinX = a.AA.x;
        float AMaxX = a.BB.x;
        float BMinX = b.AA.x;
        float BMaxX = b.BB.x;
        if (BMaxX < AMinX || BMinX > AMaxX)
            return false;
        float AMinY = a.AA.y;
        float AMaxY = a.BB.y;
        float BMinY = b.AA.y;
        float BMaxY = b.BB.y;
        if (BMaxY < AMinY || BMinY > AMaxY)
            return false;
        float AMinZ = a.AA.z;
        float AMaxZ = a.BB.z;
        float BMinZ = b.AA.z;
        float BMaxZ = b.BB.z;
        if (BMaxZ < AMinZ || BMinZ > AMaxZ)
            return false;

        return true;
    }
}

public class AABBBounding
{
    public Vector3 AA;
    public Vector3 BB;

    public AABBBounding(Vector3 aa, Vector3 bb)
    {
        AA = aa;
        BB = bb;
    }
}
