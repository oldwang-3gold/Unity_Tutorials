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
            //Turn();
            Jump();
            //Move();
            MoveAndRotation();
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
                    m_Yspeed += m_Gravity * m_DeltaTime;
                }
            }
        }

        private void Turn()
        {
            if (m_Input.x != 0 && m_Input.z != 0)
            {
                // 只有左右才会影响旋转
                float targetRotation = m_RotateSpeed * m_Input.x;
                // 绕着Y轴进行旋转
                m_Target.eulerAngles = Vector3.up * Mathf.Lerp(m_Target.eulerAngles.y, m_Target.eulerAngles.y + targetRotation, m_DeltaTime);
            }
        }

        private void Move()
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            m_Input.x = h;
            m_Input.z = v;
            // 下一个deltatime的目标速度值
            m_MoveSpeed = Mathf.MoveTowards(m_MoveSpeed, m_Input.normalized.magnitude * m_MaxSpeed, m_Acceleration * m_DeltaTime);
            m_Move = m_Input;


            // 分别处理下前后，左右涉及到转向
            if (m_Input.z != 0)
            {
                m_Move = v * m_DeltaTime * m_MoveSpeed * m_Target.forward;
                m_Move += Vector3.up * m_Yspeed * m_DeltaTime;
                m_CharacterController.Move(m_Move);
                m_Animator.SetBool("Move", true);
                m_Animator.SetFloat("InputH", 0);
                m_Animator.SetFloat("InputV", m_Input.z);
            }
            else
            {
                if (m_Input.x != 0)
                {
                    m_Move = h * m_DeltaTime * m_MoveSpeed * m_Target.right;
                    m_Move += Vector3.up * m_Yspeed * m_DeltaTime;
                    m_CharacterController.Move(m_Move);
                    m_Animator.SetBool("Move", true);
                    m_Animator.SetFloat("InputH", m_Input.x);
                    m_Animator.SetFloat("InputV", 0);
                }
                else
                {
                    // 不移动 但是可能会有跳跃和下落
                    m_Move += Vector3.up * m_Yspeed * m_DeltaTime;
                    m_CharacterController.Move(m_Move);
                    m_Animator.SetBool("Move", false);
                    m_Animator.SetFloat("InputH", m_Input.x);
                    m_Animator.SetFloat("InputV", m_Input.z);
                }
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

        private void MoveAndRotation()
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
            Vector3 desireMoveDir = cameraForward * inputZ + cameraRight * inputX;

            if (inputX != 0 || inputZ != 0)
            {
                m_Target.rotation = Quaternion.Lerp(m_Target.rotation, Quaternion.LookRotation(desireMoveDir), m_RotateSpeed * m_DeltaTime);

                desireMoveDir *= m_MoveSpeed;
                desireMoveDir += Vector3.up * m_Yspeed;
                // SimpleMove会忽视Y轴影响
                m_CharacterController.Move(desireMoveDir * m_DeltaTime);
                m_Animator.SetBool("Move", true);
                if (inputX != 0)
                {
                    m_Animator.SetFloat("InputH", m_Input.x);
                    m_Animator.SetFloat("InputV", 0);
                }
                if (inputZ != 0)
                {
                    m_Animator.SetFloat("InputH", 0);
                    m_Animator.SetFloat("InputV", m_Input.z);
                }
                
            }
            else if (inputX == 0 && inputZ == 0)
            {
                desireMoveDir += Vector3.up * m_Yspeed;
                m_CharacterController.Move(desireMoveDir * m_DeltaTime);
                m_Animator.SetBool("Move", false);
                m_Animator.SetFloat("InputH", m_Input.x);
                m_Animator.SetFloat("InputV", m_Input.z);
            }
            
        }

        public void ChangeMoveSpeed(float speed)
        {
            //m_Speed = speed;
        }
    }
}


