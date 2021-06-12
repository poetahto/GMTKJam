using UnityEngine;
using UnityEngine.Events;

// Represents the player that will move around and interact with the 
// game world. A pretty hefty class, might need to be abstracted if
// possible. 

// Best way to learn about this class is to jump right into
// the Update and FixedUpdate methods, most of the stuff there
// is explained with comments.

public class ControllableObject : MonoBehaviour
{
    [SerializeField]
    public Rigidbody body;

    [SerializeField] 
    public ControllableProperties properties;

    public UnityEvent<bool> onAttached = new UnityEvent<bool>(); 
    
    private bool OnGround => _groundContactCount > 0;
    public Vector3 Velocity => body.velocity;
    public Vector3 CameraOffset => properties.cameraOffset;
    
    private bool _desiredJump;
    private float _minNormalY;
    private int _usedJumps;
    private int _groundContactCount;
    private Rigidbody _connectedBody, _previousConnectedBody;
    private Vector3 _velocity, _desiredVelocity, _connectionVelocity;
    private Vector3 _connectionWorldPosition, _connectionLocalPosition;
    private Vector3 _contactNormal;

    private void Start()
    {
        _minNormalY = Mathf.Cos(properties.maxGroundAngle * Mathf.Deg2Rad);
    }

    private void FixedUpdate()
    {
        UpdateVelocity();    
    }

    public void SetMovementDirection(Vector2 direction)
    {
        Transform objectTransform = transform;
        
        direction.Normalize();
        Vector3 forwardMovement = objectTransform.forward * direction.x;
        Vector3 sidewaysMovement = objectTransform.right * direction.y;       
        
        _desiredVelocity = (forwardMovement + sidewaysMovement) * properties.maxSpeed;
    }

    public void TryToJump() => _desiredJump = true;

    private void UpdateVelocity()
    {
        // Ensure that our locally stored variables are updated and reflect the player state
        UpdateState();

        // Update our local velocity variable according to the players most recent inputs
        AdjustVelocity();

        // If the player wants to jump, try to jump
        if (_desiredJump)
        {
            _desiredJump = false;
            Jump();
        }

        // Update rigidbody velocity to match our local velocity variable
        body.velocity = _velocity;

        // Reset player state so it can be re-calculated in the next physics tick
        ClearState();
    }

    private void AdjustVelocity()
    {
        // Project the x and z axis along the floor we are standing on.
        // This ensures smooth movement up / down slopes.
        Vector3 xAxis = ProjectOnContactPlane(Vector3.right).normalized;
        Vector3 zAxis = ProjectOnContactPlane(Vector3.forward).normalized;

        // Make sure our velocity will be affected by any object we are standing on
        Vector3 relativeVelocity = _velocity - _connectionVelocity;

        // Adjusts our acceleration based on whether we are grounded or not
        float accel = OnGround ? properties.groundAcceleration : properties.airAcceleration;
        float maxSpeedChange = accel * Time.deltaTime;

        // Represents the x and z components of our current velocity
        float currentX = Vector3.Dot(relativeVelocity, xAxis);
        float currentZ = Vector3.Dot(relativeVelocity, zAxis);
        
        // Represents the x and z components of what will be the players new velocity
        float newX = Mathf.MoveTowards(currentX, _desiredVelocity.x, maxSpeedChange);
        float newZ = Mathf.MoveTowards(currentZ, _desiredVelocity.z, maxSpeedChange);

        // Add the change in velocity that we calculated for both the x and z axis to our internal
        // representation of the velocity (it will be applied to the player by the end of this frame)
        _velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);
    }

    private void Jump()
    {
        if (properties.maxAirJumps < 0)
            return;

        if (OnGround || _usedJumps < properties.maxAirJumps)
        {
            // QOL for making double jumps work against gravity
            if (_velocity.y < 0) _velocity.y = 0;

            // Some physics equation or something for realistic jump speed?? It works but
            // we don't really care about scientific accuracy here, so maybe change later
            float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * properties.jumpHeight);

            // Represents the player's current speed in the y direction away from the surface they are standing on
            float alignedSpeed = Vector3.Dot(_velocity, _contactNormal);
            
            // If the player already has some upward velocity...
            if (alignedSpeed > 0f)
            {
                // ... decrease our jump speed such that it cannot exceed our desired speed
                jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
            }

            // Add our jump speed to our velocity, scaled away from our contact normal
            _velocity += _contactNormal * jumpSpeed;
            _usedJumps++;
        }
    }

    private void UpdateState() 
    {
        // Update our cached local velocity variable to match the true rigidbody velocity
        _velocity = body.velocity;
        
        if (OnGround)
        {
            // Refresh jumps
            _usedJumps = 0;

            // Make sure our average contact normal is normalized. Since we added all our 
            // contact normals together, magnitude would be pretty big if we didn't do this.
            if (_groundContactCount > 1) _contactNormal.Normalize();
        }
        else
        {
            // If we are not standing on the floor, default the contact normal to point straight up.
            _contactNormal = Vector3.up;
        }

        // If we are standing on another rigidbody...
        if (_connectedBody) 
        {
            // ... and if the rigidbody should actually be able to move us...
            if (_connectedBody.isKinematic || _connectedBody.mass >= body.mass)
            { 
                // ... update our cached information about the rigidbody we are standing on.
                UpdateConnectionState();
            }
        }
    }

    private void UpdateConnectionState()
    {
        // Figure out the connected object's velocity from its change in position. 
        // Takes orbital velocity into account, allowing rotation to be tracked.

        if (_connectedBody == _previousConnectedBody)
        {
            Vector3 changeInPosition =
                _connectedBody.transform.TransformPoint(_connectionLocalPosition) - _connectionWorldPosition;
            _connectionVelocity = changeInPosition / Time.deltaTime;
        }

        _connectionWorldPosition = body.position;
        _connectionLocalPosition = _connectedBody.transform.InverseTransformPoint(
            _connectionWorldPosition
        );
    }

    private void ClearState()
    {
        _groundContactCount = 0;
        _contactNormal = _connectionVelocity = Vector3.zero;
        _previousConnectedBody = _connectedBody;
        _connectedBody = null;
    }

    private void EvaluateCollision(Collision collision)
    {
        // Loop through every point at which we collided during this physics tick
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;

            // Essentially, if the normal of our ground collision is upright
            // enough, count it as a contact with the ground

            if (normal.y >= _minNormalY)
            {
                _groundContactCount += 1;
                _contactNormal += normal;
                _connectedBody = collision.rigidbody;
            }
        }
    }

    protected void OnCollisionEnter(Collision collision)
    {
        EvaluateCollision(collision);
    }

    protected void OnCollisionStay(Collision collision)
    {
        EvaluateCollision(collision);
    }

    private Vector3 ProjectOnContactPlane(Vector3 vector)
    {
        // Returns the input vector, but it is adjusted to follow the slope of the 
        // floor that the player is standing on.
        Vector3 result = vector - _contactNormal * Vector3.Dot(vector, _contactNormal);
        return result;
    }
}
