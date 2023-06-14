using UnityEngine;

[CreateAssetMenu(fileName = "Camera", menuName = "ScriptableObjects/Camera", order = 1)]
public class CameraScriptableObject : ScriptableObject
{
    public float speed;
    public float rotateSpeed;
    public Vector3 lookAtOffset;
}
