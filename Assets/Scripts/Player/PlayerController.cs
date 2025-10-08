using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

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
    float jumpForce = 20f;
    bool canJump = true;
    bool isJumping = false;

    [Header("Look Settings")]
    bool canLook = true;
    public Vector2 LookInput { get { return lookInput; } }
    Vector2 lookInput;
    [SerializeField]float lookSensitivity = 17f;
    float cameraRotationX = 0f;
    bool canTakeDamage = true;

    PlayerHealth health;

    [Header("Sound Effects")]
    public AudioClip oof;
    private void Awake()
    {
        inputManager = GameManager.Instance.inputManager;
        rb = GetComponent<Rigidbody>();
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
        // Disable looking for gun recoil.
        canLook = false;
    }

    private void OnEnable()
    {
        inputManager.MoveInputEvent += SetMoveInput;
        inputManager.LookInputEvent += SetLookInput;
        inputManager.JumpEvent += HandleJump;
        inputManager.SprintEvent += HandleSprint;
        canTakeDamage = true;
        isJumping = false;
        canJump = true;
    }

    private void OnDisable()
    {
        inputManager.MoveInputEvent -= SetMoveInput;
        inputManager.LookInputEvent -= SetLookInput;
        inputManager.JumpEvent -= HandleJump;
        inputManager.SprintEvent -= HandleSprint;
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
        if (!isJumping) rb.angularVelocity = Vector3.zero;
    }

    private void FixedUpdate()
    {
        isGrounded = GroundCheck();
        HandleMovement();
    }

    private void Jump()
    {
        if (isGrounded && canJump)
        {
            if (rb != null)
            rb.angularVelocity = Vector3.zero;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJumping = true;
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
            isJumping = false;
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
        if (context.performed)
        {
            // When the sprint button is pressed and held, set isSprinting to true
            if (moveInput.y > 0.1f)
            isSprinting = true;
        }
        else if (context.canceled)
        {
            // When the sprint button is released, set isSprinting to false
            isSprinting = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && canTakeDamage)
        {
            canJump = false;
            Vector3 toEnemy = (collision.transform.position - transform.position).normalized;

            float dot = Vector3.Dot(transform.forward, toEnemy);

            if (health.CurrentHealth > 0 && dot > 0.7f)
            {
                TakeHit(collision);
                GameManager.Instance.audioManager.PlaySFX(oof);
                health.TakeDamage(20);
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

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            canJump = true;
        }
    }

    private void TakeHit(Collision collision)
    {
        float knockbackForce = 200;

        Vector3 knockbackDirection = -transform.forward;
        knockbackDirection.y = 0;
        knockbackDirection.Normalize();

        rb.angularVelocity = Vector3.zero;
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
    }

    IEnumerator DamageCooldown()
    {
        yield return new WaitForSeconds(1.5f);
        canTakeDamage = true;
    }


    private void HandleLook()
    {
        if (!canLook) return;
        transform.Rotate(Vector3.up * lookInput.x * lookSensitivity);

        cameraRotationX -= lookInput.y * lookSensitivity;

        // Clamp camera rotation to prevent flip effect.
        cameraRotationX = Mathf.Clamp(cameraRotationX, -90f, 90f);

        Quaternion targetRotation = Quaternion.Euler(cameraRotationX, 0f, 0f);

        playerCamera.transform.localRotation = Quaternion.Slerp(playerCamera.transform.rotation, targetRotation, 1f / Time.smoothDeltaTime);
    }

    private void HandleJump(InputAction.CallbackContext context)
    {
       if (context.performed)
        {
            Jump();
        }
    }
}
