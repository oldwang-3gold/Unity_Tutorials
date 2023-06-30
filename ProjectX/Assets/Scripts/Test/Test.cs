using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Transform target;
    public float radius = 1f;

    private float m_PrevRadius;
    private static RaycastHit[] s_RaycstHits = new RaycastHit[16];
    private static int[] s_PenetrationIndexBuffer = new int[16];
    private static SphereCollider s_ScratchCollider;
    private static GameObject s_ScratchGameObject;

    void Start()
    {
        var forward = Camera.main.transform.forward;
        var up = Camera.main.transform.up;
        var calcForward = Camera.main.transform.rotation * Vector3.forward;
        //Debug.Log($"{forward}, {calcForward}, {calcForward.magnitude}");
        Quaternion rotation = Quaternion.LookRotation(forward, up);
        //Debug.Log($"{rotation.eulerAngles}, {Quaternion.Inverse(rotation).eulerAngles}");
    }

    private void Update()
    {

        if (m_PrevRadius != radius)
        {
            m_PrevRadius = radius;
            CalcCameraPosition();
        }
    }

    void CalcCameraPosition()
    {
        Vector3 start = target.transform.position;
        Vector3 dir = Camera.main.transform.position - start;
        float len = dir.magnitude;
        dir /= len;
        RaycastHit hitInfo;
        bool r = SphereCastIgnoreTag(start, radius, dir, out hitInfo, len, 0, "Hero");
        var desiredResult = hitInfo.point + hitInfo.normal * radius;
        var desiredCorrection = (desiredResult - Camera.main.transform.position).magnitude;
        var result = Camera.main.transform.position;
        result -= dir * desiredCorrection;
        Debug.Log($"{r}, {desiredResult}, {desiredCorrection}, {result}");
    }

    bool SphereCastIgnoreTag(
        Vector3 rayStart, float radius, Vector3 dir,
        out RaycastHit hitInfo, float rayLength,
        int layerMask, in string ignoreTag)
    {
        int hits = Physics.SphereCastNonAlloc(rayStart, radius, dir, s_RaycstHits);
        int numPenetrations = 0;
        int closestHit = -1;
        float penetrationDistanceSum = 0;
        for (int i = 0; i < hits; i++)
        {
            var hit = s_RaycstHits[i];
            if (ignoreTag.Length > 0 && hit.collider.CompareTag(ignoreTag))
            {
                continue;
            }
            //Debug.Log($"{hit.collider.name}, {hit.distance}, {hit.normal}");
            if (s_PenetrationIndexBuffer.Length > numPenetrations + 1)
            {
                s_PenetrationIndexBuffer[numPenetrations++] = i;
            }
            var scratchCollider = GetScratchCollider();
            scratchCollider.radius = radius;
            var collider = hit.collider;
            if (Physics.ComputePenetration(
                scratchCollider, rayStart, Quaternion.identity,
                collider, collider.transform.position, collider.transform.rotation,
                out var offsetDir, out var offsetDistance
                ))
            {
                hit.point = rayStart + offsetDir * (offsetDistance - radius);
                hit.distance = offsetDistance - radius;
                hit.normal = offsetDir;
                s_RaycstHits[i] = hit;
                penetrationDistanceSum += hit.distance;
            }
            else
            {
                continue;
            }
            if (closestHit < 0 || hit.distance < s_RaycstHits[closestHit].distance)
            {
                closestHit = i;
            }
        }
        if (numPenetrations > 1)
        {
            hitInfo = new RaycastHit();
            for (int i = 0; i < numPenetrations; ++i)
            {
                var hit = s_RaycstHits[s_PenetrationIndexBuffer[i]];
                var t = hit.distance / penetrationDistanceSum;
                hitInfo.point += hit.point * t;
                hitInfo.distance += hit.distance * t;
                hitInfo.normal += hit.normal * t;
            }
            hitInfo.normal = hitInfo.normal.normalized;
            return true;
        }
        if (closestHit >= 0)
        {
            hitInfo = s_RaycstHits[closestHit];
            if (hits == s_RaycstHits.Length)
            {
                s_RaycstHits = new RaycastHit[s_RaycstHits.Length * 2];
            }
            return true;
        }
        hitInfo = new RaycastHit();
        return false;
    }
    
    static SphereCollider GetScratchCollider()
    {
        if (s_ScratchGameObject == null)
        {
            s_ScratchGameObject = new GameObject("Scratch Collider");
            s_ScratchGameObject.hideFlags = HideFlags.HideAndDontSave;
            s_ScratchGameObject.transform.position = Vector3.zero;
            s_ScratchGameObject.SetActive(true);
            s_ScratchCollider = s_ScratchGameObject.AddComponent<SphereCollider>();
            s_ScratchCollider.isTrigger = true;
            var rb = s_ScratchGameObject.AddComponent<Rigidbody>();
            rb.detectCollisions = false;
            rb.isKinematic = true;
        }
        return s_ScratchCollider;
    }
}
