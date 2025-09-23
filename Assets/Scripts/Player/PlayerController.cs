using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    // References.
    private InputManager inputManager;
    Rigidbody rb;   
    Camera playerCamera;

    [Header("Camera Settings")]
    Coroutine fovChangeCoroutine;
    float fovTransitionSpeed = 3.6f;
    float walkFov = 60f;
    float sprintFov = 75f;

    [Header("Movement Settings")]
    bool isSprinting = false;
    public bool isMoving;
    Vector2 moveInput;
    float movementSpeed;
    float walkSpeed = 4f;
    float runSpeed = 8f;
    bool isGrounded = false;
    float groundCheckDistance = 1.1f;

    [Header("Jump Settings")]
    float jumpForce = 450f;

    [Header("Look Settings")]
    public Vector2 LookInput { get { return lookInput; } }
    Vector2 lookInput;
    float lookSensitivity = 17f;
    float cameraRotationX = 0f;
    private void Awake()
    {
        inputManager = GameManager.Instance.inputManager;
        rb = GetComponent<Rigidbody>();
        playerCamera = GetComponentInChildren<Camera>();
        movementSpeed = walkSpeed;
    }

    private void OnEnable()
    {
        inputManager.MoveInputEvent += SetMoveInput;
        inputManager.LookInputEvent += SetLookInput;
        inputManager.JumpEvent += HandleJump;
        inputManager.SprintEvent += HandleSprint;
    }

    private void SetMoveInput(Vector2 vector)
    {
        moveInput = new Vector2(vector.x, vector.y);
    }

    private void SetLookInput(Vector2 vector)
    {
        lookInput = new Vector2(vector.x, vector.y);
    }

    private void FixedUpdate()
    {
        isGrounded = GroundCheck();
        HandleMovement();
    }

    private void Jump()
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void LateUpdate()
    {
        HandleLook();
    }

    private bool GroundCheck()
    {
        if (Physics.Raycast(transform.position, Vector3.down, groundCheckDistance))
        {
            // Something is below player.
            return true;
        }
        return false;
    }

    private void HandleMovement()
    {
        bool isMoving = moveInput.magnitude > 0.1f;

        // Determine the target FOV and speed based on sprinting and moving
        float targetFov = walkFov;
        float targetSpeed = walkSpeed;

        if (isSprinting && isMoving)
        {
            targetFov = sprintFov;
            targetSpeed = runSpeed;
        }

        // Lerp camera fov and movement speed.
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFov, Time.fixedDeltaTime * fovTransitionSpeed);
        movementSpeed = Mathf.Lerp(movementSpeed, targetSpeed, Time.fixedDeltaTime * fovTransitionSpeed);

        // Apply movement.
        Vector3 horizontalVelocity = (transform.forward * moveInput.y + transform.right * moveInput.x).normalized * movementSpeed;
        rb.linearVelocity = new Vector3(horizontalVelocity.x, rb.linearVelocity.y, horizontalVelocity.z);
    }

    private void HandleSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // When the sprint button is pressed and held, set isSprinting to true
            isSprinting = true;
        }
        else if (context.canceled)
        {
            // When the sprint button is released, set isSprinting to false
            isSprinting = false;
        }
    }

   

    private void HandleLook()
    {
        transform.Rotate(Vector3.up * lookInput.x * lookSensitivity * Time.deltaTime);

        cameraRotationX -= lookInput.y * lookSensitivity * Time.deltaTime;

        // Clamp camera rotation to prevent flip effect.
        cameraRotationX = Mathf.Clamp(cameraRotationX, -90f, 90f);

        playerCamera.transform.localRotation = Quaternion.Euler(cameraRotationX, 0f, 0f);
    }

    private void HandleJump(InputAction.CallbackContext context)
    {
       if (context.performed)
        {
            Jump();
        }
    }
}
