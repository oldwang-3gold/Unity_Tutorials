using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectX.Character;

[RequireComponent(typeof(CharacterController))]
public class Character : MonoBehaviour
{
    public CameraScriptableObject cameraValues;
    public MovementScriptableObject movementValues;

    [SerializeField]
    private Hero m_Hero;

    public void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        m_Hero = new Hero(transform);
        m_Hero.SetFollowHeroCamera(cameraValues, Camera.main);
        m_Hero.SetHeroMovement(movementValues);
    }

    public void Update()
    {
        m_Hero.Update(Time.deltaTime);
    }

    public void FixedUpdate()
    {
        m_Hero.FixedUpdate(Time.fixedDeltaTime);
    }
}


