using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathUtil
{
    public static AABBBounding GetBounding(GameObject go)
    {
        Mesh mesh = go.GetComponent<MeshFilter>().mesh;
        Vector3 AA = new Vector3(int.MaxValue, int.MaxValue, int.MaxValue);
        Vector3 BB = new Vector3(int.MinValue, int.MinValue, int.MinValue);
        for (int j = 0; j < mesh.triangles.Length / 3; j += 3)
        {
            Triangle t = new Triangle(
                go.transform.TransformPoint(mesh.vertices[mesh.triangles[j]]),
                go.transform.TransformPoint(mesh.vertices[mesh.triangles[j + 1]]),
                go.transform.TransformPoint(mesh.vertices[mesh.triangles[j + 2]])
            );
            float minX = Mathf.Min(t.p1.x, Mathf.Min(t.p2.x, t.p3.x));
            float minY = Mathf.Min(t.p1.y, Mathf.Min(t.p2.y, t.p3.y));
            float minZ = Mathf.Min(t.p1.z, Mathf.Min(t.p2.z, t.p3.z));
            AA.x = Mathf.Min(AA.x, minX);
            AA.y = Mathf.Min(AA.y, minY);
            AA.z = Mathf.Min(AA.z, minZ);

            float maxX = Mathf.Max(t.p1.x, Mathf.Max(t.p2.x, t.p3.x));
            float maxY = Mathf.Max(t.p1.y, Mathf.Max(t.p2.y, t.p3.y));
            float maxZ = Mathf.Max(t.p1.z, Mathf.Max(t.p2.z, t.p3.z));
            BB.x = Mathf.Max(BB.x, maxX);
            BB.y = Mathf.Max(BB.y, maxY);
            BB.z = Mathf.Max(BB.z, maxZ);
        }
        return new AABBBounding(AA, BB);
    }
}
