using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    // References.
    private InputManager inputManager;
    Rigidbody rb;   
    Camera playerCamera;

    [Header("MovementSettings")]
    Vector2 moveInput;
    float movementSpeed = 5f;

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
        HandleMovement();
    }

    private void LateUpdate()
    {
        HandleLook();
    }

    private void HandleMovement()
    {
        Vector3 movementDirection = (transform.forward * moveInput.y + transform.right * moveInput.x).normalized;

        rb.linearVelocity = movementDirection * movementSpeed;
    }

    private void HandleLook()
    {
        transform.Rotate(Vector3.up * lookInput.x * lookSensitivity);

        cameraRotationX -= lookInput.y * lookSensitivity;

        // Clamp camera rotation to prevent flip effect.
        cameraRotationX = Mathf.Clamp(cameraRotationX, -90f, 90f);

        playerCamera.transform.localRotation = Quaternion.Euler(cameraRotationX, 0f, 0f);
    }
}
