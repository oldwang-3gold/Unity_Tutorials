using UnityEngine;

[CreateAssetMenu(fileName = "Movement", menuName = "ScriptableObjects/Movement", order = 2)]
public class MovementScriptableObject : ScriptableObject
{
    public float maxSpeed;
    public float acceleration;
    public float rotateSpeed;
    public float minAngleSpeed;
    public float maxAngleSpeed;
    public float jumpSpeed;
}
