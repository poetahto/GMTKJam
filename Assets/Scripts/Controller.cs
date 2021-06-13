using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

// represents the player who is controlling the characters in the game

public class Controller : MonoBehaviour
{
    [Range(1f, 10f), SerializeField] 
    private float mouseSensitivity = 3f;

    [SerializeField] 
    private Camera controllerCamera;

    [Layer, SerializeField] 
    private int playerLayer;
    
    [SerializeField] 
    private ControllableObject currentlyControlling;

    [SerializeField] 
    private float transitionDuration = 0.25f;

    private RigidbodyConstraints _savedConstraints;
    private Tweener _controllableTransition;
    private Vector3 _flatForward;
    
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        
        if (currentlyControlling != null)
        {
            transform.position = currentlyControlling.objectRenderer.bounds.center + currentlyControlling.CameraOffset;
            _savedConstraints = currentlyControlling.body.constraints;
            AttachTo(currentlyControlling);
        }

        fadeIntroScreen.alpha = 1f;
        fadeIntroScreen.DOFade(0, 0.5f);
    }

    private void Update()
    {
        HandleMouseInput(!_controllableTransition.IsActive());

        var flatForward = transform.forward;
        flatForward.y = 0;
        flatForward.Normalize();
        
        if (flatForward != Vector3.zero)
            _flatForward = flatForward;
        
        if (!_controllableTransition.IsActive())
        {
            UpdateCameraPos();
            HandleKeyboardInput();
        }
    }

    private void HandleKeyboardInput()
    {
        Vector2 playerInput;
        playerInput.x = Input.GetAxisRaw("Vertical");
        playerInput.y = Input.GetAxisRaw("Horizontal");
        playerInput.Normalize();

        var playerTransform = transform;

        Vector3 forwardMovement = _flatForward * playerInput.x;
        Vector3 sidewaysMovement = playerTransform.right * playerInput.y;
        currentlyControlling.SetMovementDirection(forwardMovement + sidewaysMovement);
        
        if (Input.GetButtonDown("Jump"))
            currentlyControlling.TryToJump();
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out var hit, Mathf.Infinity
            ,~(1 << playerLayer)))
            {
                var obj = hit.transform.GetComponent<ControllableObject>();
                
                if (obj != currentlyControlling)
                    AttachTo(obj);
            }
        }

        
        if (Input.GetKey(KeyCode.Escape))
        {
            _escapeHoldSeconds += Time.deltaTime;

            if (_escapeHoldSeconds >= escapeHoldTime)
            {
                SceneManager.LoadScene(mainMenu);
                Cursor.lockState = CursorLockMode.None;
            }
        }
        else
        {
            _escapeHoldSeconds = Mathf.Max(_escapeHoldSeconds - Time.deltaTime, 0);
        }
        
        fadeScreen.alpha = Mathf.Lerp(0, 1, _escapeHoldSeconds / escapeHoldTime);
    }

    private void OnGUI()
    {
        GUILayout.Label(_escapeHoldSeconds.ToString());
    }

    private float _escapeHoldSeconds;
    [Scene, SerializeField] private string mainMenu;
    [SerializeField] private float escapeHoldTime = 5f;
    [SerializeField] private CanvasGroup fadeScreen;
    [SerializeField] private CanvasGroup fadeIntroScreen;

    private void AttachTo(ControllableObject obj)
    {
        if (obj != null)
        {
            Time.timeScale = 0.5f;
            currentlyControlling.gameObject.layer = 0;
            currentlyControlling.SetMovementDirection(Vector2.zero);
            currentlyControlling.body.constraints = _savedConstraints;
            currentlyControlling.onAttached.Invoke(false);
            currentlyControlling = obj;
            currentlyControlling.onAttached.Invoke(true);
            _controllableTransition?.Kill(true);
            currentlyControlling.gameObject.layer = playerLayer;

            for (int i = 0; i < currentlyControlling.transform.childCount; i++)
                currentlyControlling.transform.GetChild(i).gameObject.layer = playerLayer;

            _controllableTransition = controllerCamera.transform
                .DOMove(obj.objectRenderer.bounds.center + obj.CameraOffset, transitionDuration)
                .SetUpdate(true)
                .SetEase(Ease.InOutSine)
                .OnComplete(() =>
                {
                    _savedConstraints = currentlyControlling.body.constraints;
                    currentlyControlling.body.constraints = RigidbodyConstraints.FreezeRotation;
                    Time.timeScale = 1f;
                });
        }
    }

    private void UpdateCameraPos()
    {
        // Updates the camera's position every frame to smoothly follow the player around.

        Vector3 cameraPos = controllerCamera.transform.position;
        Vector3 controllerPos = currentlyControlling.objectRenderer.bounds.center;
        // float velocityChange = Mathf.Max(currentlyControlling.Velocity.magnitude * Time.deltaTime, 0.1f);
        float velocityChange = currentlyControlling.Velocity.magnitude * Time.deltaTime;
        
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
    private void HandleMouseInput(bool rotateTarget)
    {
        // calculate new rotation from mouse input and sensitivity
        Vector3 localEulerAngles = transform.localEulerAngles;
        float newRotationY = localEulerAngles.y + Input.GetAxis("Mouse X") * mouseSensitivity;
        float newRotationX = localEulerAngles.x - Input.GetAxis("Mouse Y") * mouseSensitivity;

        // clamp X rotation to between 0-90 and 270-360 because euler angles wrap at 0
        if (newRotationX > 89 && newRotationX < 271)
        {
            if (newRotationX < 180)
            {
                newRotationX = 89;
            }
            else if (newRotationX > 180)
            {
                newRotationX = 271;
            }
        }

        // update local rotation with the values we calculated, no z because we dont want to pitch or roll
        var oldAngles = currentlyControlling.transform.localEulerAngles;
        if (rotateTarget)
            currentlyControlling.transform.localEulerAngles = new Vector3(oldAngles.x, newRotationY, oldAngles.z);
        controllerCamera.transform.localEulerAngles = new Vector3(newRotationX, newRotationY, 0f);
    }
}