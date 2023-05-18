using System.Collections;
using System;
using UnityEngine;
using System.Collections.Generic;

public class SortGameObjectByAxis : IComparer<GameObject>
{
    Const.Axis axis;
    public SortGameObjectByAxis(Const.Axis axis)
    {
        this.axis = axis;
    }

    int IComparer<GameObject>.Compare(GameObject x, GameObject y)
    {
        Vector3 pos1 = x.transform.position;
        Vector3 pos2 = y.transform.position;
        if (axis == Const.Axis.X)
        {
            return (int)(pos1.x - pos2.x);
        }
        else if (axis == Const.Axis.Y)
        {
            return (int)(pos1.y - pos2.y);
        }
        else if (axis == Const.Axis.Z)
        {
            return (int)(pos1.z - pos2.z);
        }
        return 0;
    }
}

public class SortTriangleByAxis : IComparer<Triangle>
{
    Const.Axis axis;
    public SortTriangleByAxis(Const.Axis axis)
    {
        this.axis = axis;
    }

    int IComparer<Triangle>.Compare(Triangle x, Triangle y)
    {
        Triangle t1 = x;
        Triangle t2 = y;
        if (axis == Const.Axis.X)
        {
            return (int)(t1.center.x - t2.center.x);
        }
        else if (axis == Const.Axis.Y)
        {
            return (int)(t1.center.y - t2.center.y);
        }
        else if (axis == Const.Axis.Z)
        {
            return (int)(t1.center.z - t2.center.z);
        }
        return 0;
    }
}

public class Triangle
{
    public Vector3 p1, p2, p3;
    public Vector3 center;
    public Triangle(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        this.p1 = p1;
        this.p2 = p2;
        this.p3 = p3;
        this.center = (p1 + p2 + p3) / 3;
    }
}


