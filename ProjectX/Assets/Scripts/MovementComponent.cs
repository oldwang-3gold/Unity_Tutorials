using System;
using UnityEngine;
using ProjectX.Character;

namespace ProjectX.Component
{
    [Serializable]
    public class MovementComponent : ComponentBase
    {
        private Transform m_Target;

        [SerializeField]
        private float m_MaxSpeed;
        [SerializeField]
        private float m_Acceleration;
        [SerializeField]
        private float m_RotateSpeed;
        [SerializeField]
        private float m_MinAngleSpeed;
        [SerializeField]
        private float m_MaxAngleSpeed;
        [SerializeField]
        private float m_JumpSpeed;

        private Vector3 m_Input = Vector3.zero;
        private float m_MoveSpeed = 0f;
        private Vector3 m_Move = Vector3.zero;
        private CharacterController m_CharacterController;
        private Animator m_Animator;
        private float m_DeltaTime;
        private float m_Gravity = 10f;
        private float m_Yspeed;
        private bool m_IsGround = true;
        private CameraComponent m_CameraComponent;

        public MovementComponent(MovementScriptableObject values, CharacterBase owner)
        {
            m_Type = EComponent.Movement;
            owner.AddComponent(m_Type, this);
            m_Target = owner.Transform;
            m_CharacterController = m_Target.GetComponent<CharacterController>();
            m_Animator = m_Target.GetComponent<Animator>();
            m_CameraComponent = owner.GetComponent<CameraComponent>(EComponent.Camera);
            m_MaxSpeed = values.maxSpeed;
            m_Acceleration = values.acceleration;
            m_RotateSpeed = values.rotateSpeed;
            m_MinAngleSpeed = values.minAngleSpeed;
            m_MaxAngleSpeed = values.maxAngleSpeed;
            m_JumpSpeed = values.jumpSpeed;
        }

        public void UpdateMovement(float deltaTime)
        {
            m_DeltaTime = deltaTime;
            Drop();
            Jump();
            Move();
            Turn();
        }

        private void Drop()
        {
            if (!m_IsGround)
            {
                m_Yspeed -= m_Gravity * m_DeltaTime;
            }
            else
            {
                if (m_Yspeed < -1)
                {
                    m_Yspeed += m_Gravity * m_DeltaTime * m_DeltaTime;
                }
            }
        }

        private void Turn()
        {
            if (m_Input.x != 0 || m_Input.z != 0)
            {
                // 只有左右才会影响旋转
                // 旋转
                m_Target.rotation = Quaternion.Lerp(m_Target.rotation, Quaternion.LookRotation(m_Move), m_RotateSpeed * m_DeltaTime);
            }
        }

        private void Jump()
        {
            if (m_IsGround && Input.GetKeyDown(KeyCode.Space))
            {
                m_Yspeed = m_JumpSpeed;
                m_IsGround = false;
                // 动画状态机里切换到子状态机
                m_Animator.CrossFade("Jump", 0.1f);
                m_Animator.SetBool("IsGround", false);
            }
            else
            {
                if (m_CharacterController.isGrounded)
                {
                    m_IsGround = true;
                    m_Animator.SetBool("IsGround", true);
                }
            }
        }

        private void Move()
        {
            var inputX = Input.GetAxis("Horizontal");
            var inputZ = Input.GetAxis("Vertical");
            m_Input.x = inputX;
            m_Input.z = inputZ;

            var camera = m_CameraComponent.Camera;
            var cameraForward = camera.transform.forward;
            var cameraRight = camera.transform.right;

            cameraForward.y = 0;
            cameraRight.y = 0;

            m_MoveSpeed = Mathf.MoveTowards(m_MoveSpeed, m_Input.normalized.magnitude * m_MaxSpeed, m_Acceleration * m_DeltaTime);
            m_Move = cameraForward * inputZ + cameraRight * inputX;

            Vector3 desiredDir = m_Move * m_MoveSpeed + Vector3.up * m_Yspeed;
            m_Animator.SetFloat("Speed", m_MoveSpeed);
            m_Animator.SetFloat("InputH", m_Input.x);
            m_Animator.SetFloat("InputV", m_Input.z);
            // SimpleMove会忽视Y轴影响,改用Move
            m_CharacterController.Move(desiredDir * m_DeltaTime);
            if (inputX != 0 || inputZ != 0)
            {
                m_Animator.SetBool("Move", true);
            }
            else
            {
                m_Animator.SetBool("Move", false);
            }
            
        }

        public void ChangeMoveSpeed(float speed)
        {
            //m_Speed = speed;
        }
    }
}


