using System;
using System.Collections.Generic;
using UnityEngine;
using ProjectX.Component;

namespace ProjectX.Character
{
    [Serializable]
    public class Hero : CharacterBase
    {
        [SerializeField]
        private CameraComponent m_Camera;

        [SerializeField]
        private MovementComponent m_Movement;

        public Hero(Transform transform) : base(transform)
        {
        }

        public void SetFollowHeroCamera(CameraScriptableObject cameraSO, UnityEngine.Camera camera)
        {
            m_Camera = new CameraComponent(cameraSO, camera, this);
        }

        public void SetHeroMovement(MovementScriptableObject movementSO)
        {
            m_Movement = new MovementComponent(movementSO, this);
        }

        public void ChangeMoveSpeed(float speed)
        {
            m_Movement.ChangeMoveSpeed(speed);
        }

        public void Update(float deltaTime)
        {
            
        }

        public void FixedUpdate(float deltaTime)
        {
            m_Camera.UpdateCameraPosition(deltaTime);
            m_Camera.UpdateCameraRotation(deltaTime);
            m_Movement.UpdateMovement(deltaTime);
        }
    }
}
