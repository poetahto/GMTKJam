using UnityEngine;

[CreateAssetMenu]
public class ControllableProperties : ScriptableObject
{
    public Vector3 cameraOffset = Vector3.zero;
    public float maxSpeed = 1f;
    public float groundAcceleration = 1f;
    public float airAcceleration = 1f;
    public float maxAirJumps = 1f;
    public float jumpHeight = 1f;
    public float maxGroundAngle = 90f;
}