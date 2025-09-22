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
    float fovTransitionSpeed = 5f;
    float walkFov = 60f;
    float sprintFov = 75f;

    [Header("Movement Settings")]
    Vector2 moveInput;
    bool isSprinting = false;
    float movementSpeed;
    float walkSpeed = 5f;
    float runSpeed = 10f;
    bool isGrounded = false;
    float groundCheckDistance = 1.1f;

    [Header("Jump Settings")]
    float jumpForce = 450f;

    [Header("Look Settings")]
    Vector2 lookInput;
    float lookSensitivity = 0.5f;
    float cameraRotationX = 0f;
    private void Awake()
    {
        inputManager = GameManager.Instance.inputManager;
        rb = GetComponent<Rigidbody>();
        playerCamera = GetComponentInChildren<Camera>();
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
        if (isSprinting)
        {
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, sprintFov, Time.deltaTime * fovTransitionSpeed);
            movementSpeed = runSpeed;
        }
        else
        {
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, walkFov, Time.deltaTime * fovTransitionSpeed);
            movementSpeed = walkSpeed;
        }
        Vector3 horizontalVelocity = (transform.forward * moveInput.y + transform.right * moveInput.x).normalized * movementSpeed;

        rb.linearVelocity = new Vector3(horizontalVelocity.x, rb.linearVelocity.y, horizontalVelocity.z);
    }

    private void HandleLook()
    {
        transform.Rotate(Vector3.up * lookInput.x * lookSensitivity);

        cameraRotationX -= lookInput.y * lookSensitivity;

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

    private void HandleSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isSprinting = true;
        }
        if (context.canceled)
        {
            isSprinting = false;
        }
    }
}
