using DG.Tweening;
using UnityEngine;

// represents the player who is controlling the characters in the game

public class Controller : MonoBehaviour
{
    [Range(1f, 10f), SerializeField] 
    private float mouseSensitivity = 3f;

    [SerializeField] 
    private Camera controllerCamera;

    [SerializeField] 
    private ControllableObject currentlyControlling;
    
    [SerializeField]
    private float cameraMoveSpeed = 0.01f;

    private Tweener _controllableTransition;
    
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        
        if (currentlyControlling != null)
            AttachTo(currentlyControlling);
    }

    private void Update()
    {
        HandleKeyboardInput();
        HandleMouseInput();
        UpdateCameraPos();
    }

    private void HandleKeyboardInput()
    {
        Vector2 playerInput;
        playerInput.x = Input.GetAxisRaw("Vertical");
        playerInput.y = Input.GetAxisRaw("Horizontal");
        
        currentlyControlling.SetMovementDirection(playerInput);
        
        if (Input.GetButtonDown("Jump"))
            currentlyControlling.TryToJump();
    }

    private void AttachTo(ControllableObject obj)
    {
        if (obj != null)
        {
            Time.timeScale = 0.5f;
            currentlyControlling = obj;
            _controllableTransition?.Kill();
            
            _controllableTransition = controllerCamera.transform
                .DOMove(obj.transform.position + obj.CameraOffset, 0.25f)
                .SetEase(Ease.InOutSine)
                .OnComplete(() => Time.timeScale = 1f);
        }
    }

    private void UpdateCameraPos()
    {
        // Updates the camera's position every frame to smoothly follow the player around.

        Vector3 cameraPos = controllerCamera.transform.position;
        Vector3 controllerPos = currentlyControlling.transform.position;

        float velocityChange = Mathf.Max(currentlyControlling.Velocity.magnitude * Time.deltaTime, cameraMoveSpeed);

        cameraPos.x = Mathf.MoveTowards(
            cameraPos.x, 
            controllerPos.x + currentlyControlling.CameraOffset.x, 
            velocityChange);
        
        cameraPos.y = Mathf.MoveTowards(
            cameraPos.y, 
            controllerPos.y + currentlyControlling.CameraOffset.y, 
            velocityChange);
        
        cameraPos.z = Mathf.MoveTowards(
            cameraPos.z, 
            controllerPos.z + currentlyControlling.CameraOffset.z, 
            velocityChange);

        transform.position = cameraPos;
    }
    private void HandleMouseInput()
    {
        // calculate new rotation from mouse input and sensitivity
        Vector3 localEulerAngles = transform.localEulerAngles;
        float newRotationY = localEulerAngles.y + Input.GetAxis("Mouse X") * mouseSensitivity;
        float newRotationX = localEulerAngles.x - Input.GetAxis("Mouse Y") * mouseSensitivity;

        // clamp X rotation to between 0-90 and 270-360 because euler angles wrap at 0
        if (newRotationX > 90 && newRotationX < 270)
        {
            if (newRotationX < 180)
            {
                newRotationX = 90;
            }
            else if (newRotationX > 180)
            {
                newRotationX = 270;
            }
        }

        // update local rotation with the values we calculated, no z because we dont want to pitch or roll
        currentlyControlling.transform.localEulerAngles = new Vector3(0f, newRotationY, 0f);
        controllerCamera.transform.localEulerAngles = new Vector3(newRotationX, newRotationY, 0f);
    }
}