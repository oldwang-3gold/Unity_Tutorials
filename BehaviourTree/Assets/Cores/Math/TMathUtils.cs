using UnityEngine;

namespace GGGBT
{
    public class TMathUtils
    {
        public static Vector3 Vector3ZeroY(Vector3 v)
        {
            return new Vector3(v.x, 0, v.z);
        }

        public static Vector3 GetDirection2D(Vector3 start, Vector3 end)
        {
            Vector3 dir = end - start;
            dir.y = 0;
            return dir.normalized;
        }

        public static float GetDistance2D(Vector3 start, Vector3 end)
        {
            Vector3 dir = end - start;
            dir.y = 0;
            return dir.magnitude;
        }
    }
}

