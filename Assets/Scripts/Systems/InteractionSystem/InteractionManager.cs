using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionManager : MonoBehaviour
{
    [Header("Interaction Settings")]
    private LayerMask interactableLayer;
    [SerializeField] private float interactionDistance = 5f;

    // Interface reference used internally.
    private IInteractable currentFocusedInteractable;

    [SerializeField] private Transform cameraRoot;

    private InputManager inputManager => GameManager.Instance.inputManager;
    private HUD hud => GameManager.Instance.hud;

    private void Start()
    {
        interactableLayer = LayerMask.GetMask("Interactable");
    }

    private void Update()
    {
        HandleInteractionDetection();
    }

    private void HandleInteractionDetection()
    {
        if (Physics.Raycast(cameraRoot.position, cameraRoot.forward, out RaycastHit hitInfo, interactionDistance, interactableLayer))
        {
            //Debug.Log(hitInfo.transform.name);
            // Get the interactable component from the hit object.
            IInteractable hitInteractable = hitInfo.transform.GetComponent<IInteractable>();

            if (hitInteractable != null)
            {
                if (hitInteractable != currentFocusedInteractable)
                {
                    if (currentFocusedInteractable != null)
                    {
                        // Clear focus from previous interactable.
                        currentFocusedInteractable.SetFocus(false);
                    }
                    // Set new focus.
                    currentFocusedInteractable = hitInteractable;

                    currentFocusedInteractable.SetFocus(true);

                    // Use reference to show interaction prompt.
                    if (hitInteractable is BaseInteractable baseInteractable)
                    {
                        string promptText = baseInteractable.GetInteractionPrompt();
                        hud.DisplayPrompt(promptText, cameraRoot.transform.position);
                    }
                }
            }           
        }
        else if (currentFocusedInteractable != null)
        {
            // If no hit, clear focus from current interactable.
            currentFocusedInteractable?.SetFocus(false);
            // Clear reference.
            currentFocusedInteractable = null;
            // Hide interaction prompt.
            hud.HidePrompt();
        }
    }

    private void OnInteractInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (currentFocusedInteractable != null)
            {
                currentFocusedInteractable?.OnInteract();
            }
        }
    }
    private void OnEnable()
    {
        inputManager.InteractInputEvent += OnInteractInput;
    }
    private void OnDisable()
    {
        inputManager.InteractInputEvent -= OnInteractInput;
    }
}