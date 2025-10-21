using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    // References.
    InputManager inputManager;
    Rigidbody rb;   
    Coroutine fovChangeCoroutine;
    Camera playerCamera;
    public PlayerHealth health;

    [Header("Camera Settings")]
    [SerializeField] float fovTransitionSpeed = 3.6f;
    [SerializeField] float walkFov = 60f;
    [SerializeField] float sprintFov = 75f;

    [Header("Movement Settings")]
    Vector2 moveInput;
    [SerializeField] bool isSprinting = false;
    public bool isMoving;
    [SerializeField] float movementSpeed;
    [SerializeField] float walkSpeed = 3f;
    [SerializeField] float runSpeed = 20f;
    [SerializeField] bool isGrounded = false;
    [SerializeField] float groundCheckDistance = 1.1f;
    private bool isGamepadSprint = false;

    /*[Header("Jump Settings")]
    float jumpForce = 20f;
    bool canJump = true;
    bool isJumping = false;*/

    [Header("Look Settings")]
    Transform cameraHolder;
    [SerializeField] bool canLook = true;
    public Vector2 LookInput { get { return lookInput; } }
    Vector2 lookInput;
    [SerializeField] float lookSensitivity = 17f;
    float cameraRotationX = 0f;
    bool canTakeDamage = true;

    [Header("Sound Effects")]
    public AudioClip oof;

    private void Awake()
    {
        inputManager = GameManager.Instance.inputManager;
        rb = GetComponent<Rigidbody>();
        cameraHolder = GameObject.Find("CameraHolder").transform;
        playerCamera = GetComponentInChildren<Camera>();
        movementSpeed = walkSpeed;
        health = GetComponent<PlayerHealth>();
    }

    public void EnableLook()
    {
        canLook = true;
    }
    public void DisableLook()
    {
        canLook = false;     
    }

    private void OnEnable()
    {
        inputManager.MoveInputEvent += SetMoveInput;
        inputManager.LookInputEvent += SetLookInput;
        inputManager.SprintEvent += HandleSprint;
        inputManager.PauseEvent += DisableLook;
        canTakeDamage = true;
    }

    private void OnDisable()
    {
        inputManager.MoveInputEvent -= SetMoveInput;
        inputManager.LookInputEvent -= SetLookInput;
        inputManager.SprintEvent -= HandleSprint;
        if (rb != null)
        {
            cameraRotationX = 0f;
            moveInput = Vector2.zero;
            lookInput = Vector2.zero;
            if (cameraHolder != null) cameraHolder.localEulerAngles = Vector3.zero;
            movementSpeed = walkSpeed;
            // Reset player's velocity when enabled again.
            rb.angularVelocity = Vector3.zero;
            rb.linearVelocity = Vector3.zero;
        }
    }
    

    private void SetMoveInput(Vector2 vector)
    {
        moveInput = new Vector2(vector.x, vector.y);
    }

    private void SetLookInput(Vector2 vector)
    {
        lookInput = new Vector2(vector.x, vector.y);
    }

    private void Update()
    {
        if (transform.position.y <= -10f)
        {
            health.TakeDamage(health.CurrentHealth);
        }

        if (isSprinting && isGamepadSprint && moveInput.y <= 0.1f)
        {
            isSprinting = false;
            isGamepadSprint = false;
        }
    }

    private void FixedUpdate()
    {
        isGrounded = GroundCheck();
        HandleMovement();
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
        bool isMoving = Mathf.Abs(moveInput.y) > 0.1f; // Only forward/back
        bool movingForward = moveInput.y > 0.1f;

        // Determine the target FOV and speed based on sprinting and moving
        float targetFov = walkFov;
        float targetSpeed = walkSpeed;

        if (isSprinting && movingForward)
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
        bool isGamepad = context.control.device is Gamepad;
        if (context.started)
        {
            // When the sprint button is pressed and held, set isSprinting to true
            if (moveInput.y > 0.1f)
            isSprinting = true;
            isGamepadSprint = isGamepad;
        }
        else if (context.canceled)
        {
            if (!isGamepad)
            {
                isSprinting = false;
            }
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && canTakeDamage)
        {
            Vector3 toEnemy = (collision.transform.position - transform.position).normalized;

            float dot = Vector3.Dot(transform.forward, toEnemy);

            if (health.CurrentHealth > 0 && dot > 0.7f)
            {
                health.TakeDamage(20);
                GameManager.Instance.audioManager.PlaySFX(oof);
                canTakeDamage = false;
                if (isActiveAndEnabled)
                {
                    StartCoroutine(DamageCooldown());
                }
            }
            else
            {
                Debug.Log("Ignore collision, not facing enemy or dead.");
            }
        }
    }

    IEnumerator DamageCooldown()
    {
        yield return new WaitForSeconds(1.5f);
        canTakeDamage = true;
    }


    private void HandleLook()
    {
        if (!canLook) return;

        float horizontalLook = lookInput.x * lookSensitivity;
        float verticalLook = lookInput.y * lookSensitivity;

        transform.Rotate(Vector3.up * horizontalLook);
        Vector3 angles = cameraHolder.localEulerAngles;
        float newRotX = angles.x - verticalLook;
        newRotX = (newRotX > 180) ? newRotX - 360 : newRotX;
        newRotX = Mathf.Clamp(newRotX, -60f, 60f);

        cameraHolder.localEulerAngles = new Vector3(newRotX, 0f, 0f);
    }
}
