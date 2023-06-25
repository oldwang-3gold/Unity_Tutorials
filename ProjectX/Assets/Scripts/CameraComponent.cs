using System;
using UnityEngine;
using ProjectX.Character;

namespace ProjectX.Component
{
    [Serializable]
    public class CameraComponent : ComponentBase
    {
        [SerializeField]
        private float m_Speed;
        [SerializeField]
        private float m_RotateSpeed;
        [SerializeField]
        private Vector3 m_LookAtOffset;
        [SerializeField]
        private Vector2 m_UpClampedAngle;
        [SerializeField]
        private float m_CameraRadius;

        private Vector3 m_Offset;
        private UnityEngine.Camera m_Camera;
        public UnityEngine.Camera Camera { get { return m_Camera; } }
        private Transform m_FollowTarget;
        private float m_Distance;
        private float m_MaxDistance;
        private string m_IgnoreTag = "Hero";
        private static RaycastHit[] s_RaycastHits = new RaycastHit[16];

        public CameraComponent(CameraScriptableObject values, UnityEngine.Camera camera, CharacterBase owner)
        {
            m_Type = EComponent.Camera;
            owner.AddComponent(m_Type, this);
            m_Speed = values.speed;
            m_RotateSpeed = values.rotateSpeed;
            m_LookAtOffset = values.lookAtOffset;
            m_UpClampedAngle = values.upClampedAngle;
            m_CameraRadius = values.cameraRadius;
            m_Camera = camera;
            m_FollowTarget = owner.Transform;

            InitCameraPosition();
        }

        private void InitCameraPosition()
        {
            m_Offset = m_Camera.transform.position - m_FollowTarget.position;
            m_Distance = m_Offset.magnitude;
            m_MaxDistance = m_Distance;
            m_Camera.transform.LookAt(m_FollowTarget.position);
        }

        public void UpdateCameraPosition(float deltaTime)
        {
            CheckCameraBlocked(deltaTime);
            Vector3 targetCameraPos;
            if (Input.GetKey(KeyCode.Mouse1))
            {
                // 控制相机的移动，根据鼠标上下左右平移，围绕着目标点进行移动
                float h = Input.GetAxis("Mouse X");
                float v = Input.GetAxis("Mouse Y");
                Vector3 desiredDir = m_Camera.transform.right * h + m_Camera.transform.up * v;
                // 本质其实是修改的是m_Offset
                m_Offset = (m_Camera.transform.position + desiredDir * m_RotateSpeed * deltaTime - m_FollowTarget.position).normalized * m_Distance;
            }
            else
            {
                m_Offset = (m_Camera.transform.position - m_FollowTarget.position).normalized * m_Distance;
            }
            targetCameraPos = m_FollowTarget.position + m_Offset;
            m_Camera.transform.position = Vector3.Lerp(m_Camera.transform.position, targetCameraPos, deltaTime * m_Speed);
        }

        private void CheckCameraBlocked(float deltaTime)
        {
            Vector3 start = m_FollowTarget.position + Vector3.up;
            Vector3 dir = m_Camera.transform.position - m_FollowTarget.position - Vector3.up;
            float len = dir.magnitude;
            dir /= len;
            Ray ray = new Ray(start, dir.normalized);
            Debug.DrawRay(start, dir, Color.red);
            //if (Physics.Raycast(ray, out RaycastHit hitInfo, m_MaxDistance, m_Mask))
            //{
            //    m_Distance = Mathf.Min((ray.GetPoint(hitInfo.distance - 0.3f) - m_FollowTarget.position).magnitude, m_MaxDistance);
            //    Debug.Log($"碰撞对象: {hitInfo.collider.name}, 修改距离:{m_Distance}, {m_MaxDistance}");

            int numHits = Physics.SphereCastNonAlloc(start, m_CameraRadius, dir, s_RaycastHits, len);
            for (int i = 0; i < numHits; i++)
            {
                var hit = s_RaycastHits[i];
                if (m_IgnoreTag.Length > 0 && hit.collider.CompareTag(m_IgnoreTag))
                    continue;

                // 有碰撞
                if (hit.distance == 0 && hit.normal == -dir)
                {

                }
            }

            m_Distance = Mathf.Lerp(m_Distance, m_MaxDistance, m_Speed * deltaTime);
  
        }

        public void UpdateCameraRotation(float deltaTime)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                // 控制相机的移动
                float h = Input.GetAxis("Mouse X");
                float v = Input.GetAxis("Mouse Y");
                Vector3 input = new Vector3(h, 0, v);
                
            }
            m_Camera.transform.LookAt(m_FollowTarget.position);
        }
    }
}

