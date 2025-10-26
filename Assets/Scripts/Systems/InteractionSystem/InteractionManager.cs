using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class InteractionManager : MonoBehaviour
{
    public Interactable currentInteractable = null;
    private Camera mainCamera;
    private Outline currentOutline;
    public float interactionDistance = 1000f;
    Vector2 mousePosition;

    private void Awake()
    {
        mainCamera = GameObject.Find("CameraHolder").GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        CheckForInteractable();
        HandleInteraction();
    }

    private void CheckForInteractable()
    {
        // Get mouse position and create ray from camera
        mousePosition = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        // Cast ray from mouse position
        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            // Check if it's an interactable
            if (hit.transform.CompareTag("Interactable"))
            {
                Interactable interactable = hit.transform.GetComponent<Interactable>();

                // If this is a new interactable, update it
                if (interactable != currentInteractable)
                {
                    ClearCurrentInteractable();

                    currentInteractable = interactable;
                    currentOutline = currentInteractable.GetComponent<Outline>();

                    if (currentOutline != null)
                    {
                        currentOutline.enabled = true;
                    }

                    Debug.Log("Looking at: " + currentInteractable.name);
                }
                return;
            }
        }

        // If we didn't hit an interactable, clear the current one
        ClearCurrentInteractable();
    }

    private void HandleInteraction()
    {
        // Can use E key or mouse click
        if (currentInteractable != null &&
            (Keyboard.current.eKey.wasPressedThisFrame ||
             Mouse.current.leftButton.wasPressedThisFrame))
        {
            currentInteractable.Interact();
        }
    }

    private void ClearCurrentInteractable()
    {
        if (currentOutline != null)
        {
            currentOutline.enabled = false;
        }

        currentInteractable = null;
        currentOutline = null;
    }

   private void OnDrawGizmos()
{
    if (mainCamera == null) return;
    
    // Convert mouse position to a ray
    Ray ray = mainCamera.ScreenPointToRay(mousePosition);
    
    // Draw the ray in world space
    Gizmos.color = Color.red;
    Gizmos.DrawLine(ray.origin, ray.origin + ray.direction * interactionDistance);
}
}