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

        private Vector3 m_Offset;
        private UnityEngine.Camera m_Camera;
        public UnityEngine.Camera Camera { get { return m_Camera; } }
        private Transform m_FollowTarget;

        public CameraComponent(CameraScriptableObject values, UnityEngine.Camera camera, CharacterBase owner)
        {
            m_Type = EComponent.Camera;
            owner.AddComponent(m_Type, this);
            m_Speed = values.speed;
            m_RotateSpeed = values.rotateSpeed;
            m_LookAtOffset = values.lookAtOffset;
            m_Camera = camera;
            m_FollowTarget = owner.Transform;

            InitCameraPosition();
        }

        private void InitCameraPosition()
        {
            m_Offset = m_Camera.transform.position - m_FollowTarget.position;
            m_Camera.transform.LookAt(m_FollowTarget.position + m_LookAtOffset);
        }

        public void UpdateCameraPosition(float deltaTime)
        {
            Vector3 targetCameraPos = m_FollowTarget.position + m_Offset;
            m_Camera.transform.LookAt(m_FollowTarget.position + m_LookAtOffset);
            m_Camera.transform.position = Vector3.Lerp(m_Camera.transform.position, targetCameraPos, deltaTime * m_Speed);
        }

        public void UpdateCameraRotation(float deltaTime)
        {

            float h = Input.GetAxis("Mouse X") * m_RotateSpeed;
            //m_FollowTarget.transform.eulerAngles = 
            m_Camera.transform.LookAt(m_FollowTarget.position + m_LookAtOffset);
        }
    }
}

