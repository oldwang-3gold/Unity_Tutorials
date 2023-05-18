using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitResult
{
    public int index = -1;
    public GameObject hitObject;

    public HitResult(GameObject go)
    {
        hitObject = go;
    }
}

public class BVHTree
{
    public BVHNode root;

    private List<Triangle> triangles;
    private List<GameObject> allGo;

    public BVHTree(List<Triangle> triangles)
    {
        if (triangles != null)
        {
            root = BVHNode.Build(triangles, 0, triangles.Count - 1, 2, 0);
        }
        this.triangles = triangles;
    }

    public BVHTree(List<GameObject> allGo)
    {
        if (allGo != null)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            root = BVHNode.Build(allGo, 0, allGo.Count - 1, 2, 0);
            //root = BVHNode.BuildWithSAH(allGo, 0, allGo.Count - 1, 2, 0);
            sw.Stop();
            Debug.Log($"Cost time: {sw.ElapsedMilliseconds}");
        }
        this.allGo = allGo;
    }

    public void Update()
    {
        Debug.Log("Update BVH Tree");
        root = BVHNode.Build(allGo, 0, allGo.Count - 1, 2, 0);
    }

    public void Search(BVHNode node, AABBBounding bound, ref List<HitResult> results)
    {
        if (node == null) return;
        // р╤вс╫з╣Ц
        if (node.n > 0)
        {
            for (int i = node.index; i <= node.n + node.index; i++)
            {
                GameObject go = allGo[i];
                if (bound.CheckAABB(MathUtil.GetBounding(go)))
                {
                    results.Add(new HitResult(go));
                }
            }
        }

        if (node.left != null)
        {
            if (bound.CheckAABB(node.left.GetBounding()))
            {
                Search(node.left, bound, ref results);
            }
        }
        if (node.right != null)
        {
            if (bound.CheckAABB(node.right.GetBounding()))
            {
                Search(node.right, bound, ref results);
            }
        }
    }
}
