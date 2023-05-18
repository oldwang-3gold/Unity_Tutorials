using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    private Vector3 curPosition = Vector3.zero;

    void Update()
    {
        if (curPosition == Vector3.zero)
        {
            curPosition = transform.position;
        }
        if (Vector3.Distance(transform.position, curPosition) > 0.01f)
        {
            curPosition = transform.position;
            Example.Instance.tree.Update();
        }
    }
}
