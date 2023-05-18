using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BVHNode
{
    public BVHNode left = null;
    public BVHNode right = null;
    public int n;                  // 节点存储的对象个数
    public int index;              // 对应BVHTree中起始[index, index + n]范围的对象都属于该节点下
    int depth = 0;
    Vector3 AA, BB;
    AABBBounding bound = null;

    public Vector3 GetCenter()
    {
        return (BB + AA) / 2;
    }

    public Vector3 GetSize()
    {
        return new Vector3(
            Mathf.Abs(BB.x - AA.x),
            Mathf.Abs(BB.y - AA.y),
            Mathf.Abs(BB.z - AA.z)
        );
    }

    public AABBBounding GetBounding()
    {
        if (bound == null)
        {
            bound = new AABBBounding(AA, BB);
        }
        return bound;
    }

    public static BVHNode Build(List<Triangle> triangles, int l, int r, int n, int depth)
    {
        if (l > r) return null;
        BVHNode node = new BVHNode();
        node.depth = depth;
        node.AA = new Vector3(int.MaxValue, int.MaxValue, int.MaxValue);
        node.BB = new Vector3(int.MinValue, int.MinValue, int.MinValue);

        // 计算AABB
        for (int i = l; i <= r; i++)
        {
            Triangle t = triangles[i];
            float minX = Mathf.Min(t.p1.x, Mathf.Min(t.p2.x, t.p3.x));
            float minY = Mathf.Min(t.p1.y, Mathf.Min(t.p2.y, t.p3.y));
            float minZ = Mathf.Min(t.p1.z, Mathf.Min(t.p2.z, t.p3.z));
            node.AA.x = Mathf.Min(node.AA.x, minX);
            node.AA.y = Mathf.Min(node.AA.y, minY);
            node.AA.z = Mathf.Min(node.AA.z, minZ);

            float maxX = Mathf.Max(t.p1.x, Mathf.Max(t.p2.x, t.p3.x));
            float maxY = Mathf.Max(t.p1.y, Mathf.Max(t.p2.y, t.p3.y));
            float maxZ = Mathf.Max(t.p1.z, Mathf.Max(t.p2.z, t.p3.z));
            node.BB.x = Mathf.Max(node.BB.x, maxX);
            node.BB.y = Mathf.Max(node.BB.y, maxY);
            node.BB.z = Mathf.Max(node.BB.z, maxZ);
        }

        if ((r - l + 1) <= n)
        {
            // 叶子结点
            node.n = r - l + 1;
            node.index = l;
            return node;
        }

        // 递归建树
        float lenX = node.BB.x - node.AA.x;
        float lenY = node.BB.y - node.AA.y;
        float lenZ = node.BB.z - node.AA.z;

        if (lenX >= lenY && lenX >= lenZ)
        {
            triangles.Sort(l, r - l + 1, new SortTriangleByAxis(Const.Axis.X));
        }
        if (lenY >= lenX && lenY >= lenZ)
        {
            triangles.Sort(l, r - l + 1, new SortTriangleByAxis(Const.Axis.Y));
        }
        if (lenZ >= lenX && lenZ >= lenY)
        {
            triangles.Sort(l, r - l + 1, new SortTriangleByAxis(Const.Axis.Z));
        }

        int mid = (l + r) / 2;
        node.left = Build(triangles, l, mid, n, depth + 1);
        node.right = Build(triangles, mid + 1, r, n, depth + 1);

        return node;
    }

    public static BVHNode Build(List<GameObject> allGo, int l, int r, int n, int depth)
    {
        if (l > r) return null;
        BVHNode node = new BVHNode();
        node.depth = depth;
        node.AA = new Vector3(int.MaxValue, int.MaxValue, int.MaxValue);
        node.BB = new Vector3(int.MinValue, int.MinValue, int.MinValue);

        // 计算AABB
        for (int i = l; i <= r; i++)
        {
            GameObject go = allGo[i];
            Mesh mesh = go.GetComponent<MeshFilter>().mesh;
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
                node.AA.x = Mathf.Min(node.AA.x, minX);
                node.AA.y = Mathf.Min(node.AA.y, minY);
                node.AA.z = Mathf.Min(node.AA.z, minZ);

                float maxX = Mathf.Max(t.p1.x, Mathf.Max(t.p2.x, t.p3.x));
                float maxY = Mathf.Max(t.p1.y, Mathf.Max(t.p2.y, t.p3.y));
                float maxZ = Mathf.Max(t.p1.z, Mathf.Max(t.p2.z, t.p3.z));
                node.BB.x = Mathf.Max(node.BB.x, maxX);
                node.BB.y = Mathf.Max(node.BB.y, maxY);
                node.BB.z = Mathf.Max(node.BB.z, maxZ);
            }
            
        }

        if ((r - l + 1) <= n)
        {
            // 叶子结点
            node.n = r - l + 1;
            node.index = l;
            return node;
        }

        // 递归建树
        float lenX = node.BB.x - node.AA.x;
        float lenY = node.BB.y - node.AA.y;
        float lenZ = node.BB.z - node.AA.z;

        if (lenX >= lenY && lenX >= lenZ)
        {
            allGo.Sort(l, r - l + 1, new SortGameObjectByAxis(Const.Axis.X));
        }
        if (lenY >= lenX && lenY >= lenZ)
        {
            allGo.Sort(l, r - l + 1, new SortGameObjectByAxis(Const.Axis.Y));
        }
        if (lenZ >= lenX && lenZ >= lenY)
        {
            allGo.Sort(l, r - l + 1, new SortGameObjectByAxis(Const.Axis.Z));
        }

        int mid = (l + r) / 2;
        node.left = Build(allGo, l, mid, n, depth + 1);
        node.right = Build(allGo, mid + 1, r, n, depth + 1);

        return node;
    }

    public static BVHNode BuildWithSAH(List<GameObject> allGo, int l, int r, int n, int depth)
    {
        if (l > r) return null;
        BVHNode node = new BVHNode();
        node.depth = depth;
        node.AA = new Vector3(int.MaxValue, int.MaxValue, int.MaxValue);
        node.BB = new Vector3(int.MinValue, int.MinValue, int.MinValue);

        // 计算AABB
        for (int i = l; i <= r; i++)
        {
            GameObject go = allGo[i];
            AABBBounding bound = MathUtil.GetBounding(go);
            node.AA = Vector3.Min(node.AA, bound.AA);
            node.BB = Vector3.Max(node.BB, bound.BB);
        }

        if ((r - l + 1) <= n)
        {
            // 叶子结点
            node.n = r - l + 1;
            node.index = l;
            return node;
        }

        // 递归建树
        float Cost = int.MaxValue;
        int Axis = 0;
        int Split = (l + r) / 2;
        for (int axis = (int)Const.Axis.X; axis < (int)Const.Axis.Length; axis++)
        {
            if (axis == (int)Const.Axis.X)
            {
                allGo.Sort(l, r - l + 1, new SortGameObjectByAxis(Const.Axis.X));
            }
            if (axis == (int)Const.Axis.Y)
            {
                allGo.Sort(l, r - l + 1, new SortGameObjectByAxis(Const.Axis.Y));
            }
            if (axis == (int)Const.Axis.Z)
            {
                allGo.Sort(l, r - l + 1, new SortGameObjectByAxis(Const.Axis.Z));
            }

            List<Vector3> leftMax = new List<Vector3>(r - l + 1);
            List<Vector3> leftMin = new List<Vector3>(r - l + 1);
            for (int i = 0; i < r - l + 1; i++)
            {
                leftMax.Add(new Vector3(int.MinValue, int.MinValue, int.MinValue));
                leftMin.Add(new Vector3(int.MaxValue, int.MaxValue, int.MaxValue));
            }
            // 计算前缀
            for (int i = l; i <= r; i++)
            {
                int bias = (i == l) ? 0 : 1;
                GameObject go = allGo[i];
                AABBBounding bound = MathUtil.GetBounding(go);
                leftMax[i - l] = Vector3.Max(leftMax[i - l - bias], bound.BB);
                leftMin[i - l] = Vector3.Min(leftMin[i - l - bias], bound.AA);
            }

            List<Vector3> rightMax = new List<Vector3>(r - l + 1);
            List<Vector3> rightMin = new List<Vector3>(r - l + 1);
            for (int i = 0; i < r - l + 1; i++)
            {
                rightMax.Add(new Vector3(int.MinValue, int.MinValue, int.MinValue));
                rightMin.Add(new Vector3(int.MaxValue, int.MaxValue, int.MaxValue));
            }
            // 计算后缀
            for (int i = r; i >= l; i--)
            {
                int bias = (i == r) ? 0 : 1;
                GameObject go = allGo[i];
                AABBBounding bound = MathUtil.GetBounding(go);
                rightMax[i - l] = Vector3.Max(rightMax[i - l + bias], bound.BB);
                rightMin[i - l] = Vector3.Min(rightMin[i - l + bias], bound.AA);
            }

            float cost = int.MaxValue;
            int split = l;
            for (int i = l; i <= r - 1; i++)
            {
                float lenX, lenY, lenZ;
                Vector3 leftAA = leftMin[i - l];
                Vector3 leftBB = leftMax[i - l];
                lenX = leftBB.x - leftAA.x;
                lenY = leftBB.y - leftAA.y;
                lenZ = leftBB.z - leftAA.z;
                float leftS = 2f * ((lenX * lenY) + (lenX * lenZ) + (lenY * lenZ));
                float leftCost = leftS * (i - l + 1);

                Vector3 rightAA = rightMin[i + 1 - l];
                Vector3 rightBB = rightMax[i + 1 - l];
                lenX = rightBB.x - rightAA.x;
                lenY = rightBB.y - rightAA.y;
                lenZ = rightBB.z - rightAA.z;
                float rightS = 2f * ((lenX * lenY) + (lenX * lenZ) + (lenY * lenZ));
                float rightCost = rightS * (r - i);

                float totalCost = leftCost + rightCost;
                if (totalCost < cost)
                {
                    cost = totalCost;
                    split = i;
                }
            }

            if (cost < Cost)
            {
                Cost = cost;
                Axis = axis;
                Split = split;
            }
        }
        
        if (Axis == (int)Const.Axis.X)
        {
            allGo.Sort(l, r - l + 1, new SortGameObjectByAxis(Const.Axis.X));
        }
        if (Axis == (int)Const.Axis.Y)
        {
            allGo.Sort(l, r - l + 1, new SortGameObjectByAxis(Const.Axis.Y));
        }
        if (Axis == (int)Const.Axis.Z)
        {
            allGo.Sort(l, r - l + 1, new SortGameObjectByAxis(Const.Axis.Z));
        }

        node.left = BuildWithSAH(allGo, l, Split, n, depth + 1);
        node.right = BuildWithSAH(allGo, Split + 1, r, n, depth + 1);

        return node;
    }
}
