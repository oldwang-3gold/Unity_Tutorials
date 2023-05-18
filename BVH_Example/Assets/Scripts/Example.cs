using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Example : MonoBehaviour
{
    public BVHTree tree = null;
    public bool debugShowNode = false;
    [Range(0f, 8f)]
    public int showMaxDepth = 5;

    public GameObject target;

    public List<GameObject> tests;

    private List<HitResult> hitResults = new List<HitResult>();

    public static Example Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        BuildByGameObjects();
    }

    public void Update()
    {
        if (tree == null) return;
        ClearHitResults();
        hitResults = new List<HitResult>();
        tree.Search(tree.root, MathUtil.GetBounding(target), ref hitResults);
        ShowHitResults();
    }

    void ClearHitResults()
    {
        foreach(var hit in hitResults)
        {
            hit.hitObject.GetComponent<MeshRenderer>().material.color = Color.white;
        }
    }

    void ShowHitResults()
    {
        foreach (var hit in hitResults)
        {
            hit.hitObject.GetComponent<MeshRenderer>().material.color = Color.red;
        }
    }

    void BuildByGameObjects()
    {
        var cubes = GameObject.FindGameObjectsWithTag("Cube");
        List<GameObject> allGo = new List<GameObject>(cubes);
        tree = new BVHTree(allGo);
        tests = allGo;
    }

    void BuildByTriangles()
    {
        var cubes = GameObject.FindGameObjectsWithTag("Cube");
        List<Triangle> triangles = new List<Triangle>();
        foreach (var cube in cubes)
        {
            Mesh mesh = cube.GetComponent<MeshFilter>().mesh;
            for (int i = 0; i < mesh.triangles.Length / 3; i += 3)
            {
                Triangle tri = new Triangle(
                    cube.transform.TransformPoint(mesh.vertices[mesh.triangles[i]]),
                    cube.transform.TransformPoint(mesh.vertices[mesh.triangles[i + 1]]),
                    cube.transform.TransformPoint(mesh.vertices[mesh.triangles[i + 2]])
                );
                //Debug.Log($"Triangle, v1:{cube.transform.TransformPoint(mesh.vertices[mesh.triangles[i]])}, " +
                //    $"v2:{cube.transform.TransformPoint(mesh.vertices[mesh.triangles[i + 1]])}, " +
                //    $"v3:{cube.transform.TransformPoint(mesh.vertices[mesh.triangles[i + 2]])}");
                triangles.Add(tri);
            }
        }
        tree = new BVHTree(triangles);
    }

    public void OnDrawGizmos()
    {
        if (tree == null) return;
        BVHNode node = tree.root;
        DrawNodes(node, 0);
    }

    void DrawNodes(BVHNode node, int depth)
    {
        if (node == null) return;
        if (depth >= showMaxDepth) return;
        Gizmos.color = new Color(1 - depth * 0.15f, 0, depth * 0.15f);
        Gizmos.DrawWireCube(node.GetCenter(), node.GetSize());
        if (node.left != null) DrawNodes(node.left, depth + 1);
        if (node.right != null) DrawNodes(node.right, depth + 1);
    }
}
